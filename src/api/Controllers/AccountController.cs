using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MimeKit;
using WishList.Api.DataAccess;
using WishList.Api.Model;
using WishList.Api.Model.Extensions;
using WishList.Api.Security;

namespace WishList.Api.Controllers;



//Account (unauthorized)
//Register account
//Forgot password
//Login

//Account (authorized)
//Update password
//Update profile
[ApiController]
[Route("account")]
[Produces("application/json")]
public class AccountController(IConfiguration config, ILogger<AccountController> logger) : Controller
{
	private readonly IConfiguration config = config;
	private readonly ILogger<AccountController> logger = logger;

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<ActionResult> Register([FromBody] RegisterUserParameters parameters)
	{
		//TODO: Kolla att användaren inte redan är registrerad

		var passwordHasher = new PasswordHasher<string>();
		var hash = passwordHasher.HashPassword(parameters.Email!, parameters.Password!);

		//Verify?
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, hash, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			throw new InvalidOperationException("Password verification failed!");


		//Spara till databas
		using var conn = DbHelper.OpenConnection(config);
		await conn.ExecuteAsync("insert into User (Name, Email, Password) values (@Name, @Email, @hash)", new { parameters.Name, parameters.Email, hash });
		logger.LogInformation("Registered user {Name} with email {Email}", parameters.Name, parameters.Email);

		return Ok();
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginParameters parameters)
	{
		//Hämta användaren
		using var conn = DbHelper.OpenConnection(config);
		var dbUser = await conn.GetDbUserByEmail(parameters.Email);
		if (dbUser is null || !dbUser.Verified)
			return BadRequest(new ProblemDetails { Detail = "Invalid email or password" });

		//Validera lösenordet
		var passwordHasher = new PasswordHasher<string>();
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, dbUser.Password!, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			return BadRequest(new ProblemDetails { Detail = "Invalid email or password" });

		//Generera och returnera JWT-token
		var expires = DateTime.Now.AddSeconds(1800); //Bra att den bara är giltig i 30 sek!
		var (_, accessToken) = JwtHelper.GenerateToken(config, dbUser.Id, expires);

		return Json(new LoginResponse
		{
			Auth = new Oauth2AuthResponse(accessToken, 1800),
			User = Model.User.Create(dbUser)
		});
	}

	[HttpPost("refreshlogin")]
	[Authorize]
	public ActionResult<Oauth2AuthResponse> RefreshLogin()
	{
		var userId = User.GetUserId();
		if (userId <= 0)
			return Unauthorized();

		var expires = DateTime.Now.AddSeconds(1800);
		var (_, accessToken) = JwtHelper.GenerateToken(config, userId, expires);
		return Json(new Oauth2AuthResponse(accessToken, 1800));
	}

	[HttpPost("resetpwd/{email}")]
	[AllowAnonymous]
	public async Task<ActionResult> ResetPassword(string email)
	{
		using var conn = DbHelper.OpenConnection(config);
		var dbUser = await conn.GetDbUserByEmail(email);
		if (dbUser is null || !dbUser.Verified)
			return BadRequest(new ProblemDetails { Detail = "No such user" });

		//Create token by hashing a guid and onverting to base64 string
		var guid = Guid.NewGuid();
		var token = Convert.ToBase64String(SHA1.HashData(Encoding.UTF8.GetBytes(guid.ToString())));
		token = token.Replace('+', '-'); //Pga url...
		await conn.ExecuteAsync("update User set PwdResetToken = @token, PwdResetExpires = @expires where Id = @Id",
								new { token, expires = DateTime.Now.AddHours(24), dbUser.Id });

		var resetUrl = $"{config["WebUrl"]}/resetpassword?token={token}";
		var textBody = @$"Hej!

En begäran om att återställa ditt lösenord på Önskelistemaskinen har gjorts.

För att återställa lösenordet, gå till {resetUrl}

Om du inte begärt en återställning av lösenordet, bortse från detta mail.";

		Console.WriteLine(textBody);


		//TODO: Bryt ut detta till något återanvändbart
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("Önskelistemaskninen", "no-reply@wish.driessen.se"));
		message.To.Add(new MailboxAddress(dbUser.Name, dbUser.Email));
		message.Subject = "Begäran om att återställa lösenord";
		message.Body = new BodyBuilder { TextBody = textBody }.ToMessageBody();

		using var client = new SmtpClient();
		client.Connect(config["Mail:Host"], config.GetValue("Mail:Port", 25), MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
		client.Send(message);
		client.Disconnect(true);

		logger.LogInformation("User {userId} ({email}) requested a password reset from IP {ipAdress}", dbUser.Id, dbUser.Email, Request.HttpContext.Connection.RemoteIpAddress);

		return Ok();
	}

	[HttpPost("validateresettoken")]
	[AllowAnonymous]
	public async Task<ActionResult> ValidateResetToken([FromQuery] string token)
	{
		//Hämta user på token
		using var conn = DbHelper.OpenConnection(config);
		var dbUser = await conn.GetDbUserByResetPwdToken(token);
		if (dbUser is null || dbUser.PwdResetExpires is null || dbUser.PwdResetExpires < DateTime.Now)
			return BadRequest(new ProblemDetails { Detail = "Invalid token" });

		return Ok();
	}

	[HttpPost("resetpwd")]
	[AllowAnonymous]
	public async Task<ActionResult> ResetPasswordWithToken([FromBody] ResetPasswordParameters parameters)
	{
		//Hämta user på token
		using var conn = DbHelper.OpenConnection(config);
		var dbUser = await conn.GetDbUserByResetPwdToken(parameters.Token);
		if (dbUser is null || dbUser.PwdResetExpires is null || dbUser.PwdResetExpires < DateTime.Now)
			return BadRequest(new ProblemDetails { Detail = "Invalid token" });

		if (!parameters.Password.IsValidPassword())
			return BadRequest(new ProblemDetails { Detail = "Invalid password" });

		//Uppdatera lösenord
		var passwordHasher = new PasswordHasher<string>();
		var hash = passwordHasher.HashPassword(dbUser.Email!, parameters.Password!);
		await conn.ExecuteAsync("update User set Password = @hash, PwdResetToken = null, PwdResetExpires = null where Id = @Id", new { dbUser.Id, hash });

		logger.LogInformation("Password for user {userId} ({email}) was reset from IP {ipAdress}", dbUser.Id, dbUser.Email, Request.HttpContext.Connection.RemoteIpAddress);

		return Ok();
	}

	[HttpPost("settings")]
	public async Task<ActionResult<Model.User>> UpdateUserSettings([FromBody] UpdateSettingsParameters parameters)
	{
		var userId = User.GetUserId();
		using var conn = DbHelper.OpenConnection(config);

		var dbUser = await conn.GetDbUserById(userId);
		if (dbUser is null)
			return Unauthorized();

		//Kolla att användarnamnet inte redan är upptaget av någon annan
		var users = await conn.GetUserList();
		if (users.Any(x => x.Id != userId && x.Name?.Equals(parameters.Name, StringComparison.InvariantCultureIgnoreCase) == true))
		{
			return new ValidationErrorResult(errors: new() { { "Name", "There is already a user with that name." } });
		}

		await conn.ExecuteAsync("update User set Name = @Name, Email = @Email, Notify = @Notify where Id = @userId",
			new { userId, parameters.Name, parameters.Email, parameters.Notify });

		dbUser = await conn.GetDbUserById(userId);

		return Json(Model.User.Create(dbUser!));
	}

	[HttpPost("password")]
	public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordParameters parameters)
	{

		var userId = User.GetUserId();
		using var conn = DbHelper.OpenConnection(config);

		var dbUser = await conn.GetDbUserById(userId);
		if (dbUser is null)
			return Unauthorized();

		var passwordHasher = new PasswordHasher<string>();
		var hash = passwordHasher.HashPassword(dbUser.Email!, parameters.Password!);
		await conn.ExecuteAsync("update User set Password = @hash, PwdResetToken = null, PwdResetExpires = null where Id = @Id", new { dbUser.Id, hash });

		logger.LogInformation("Password for user {userId} ({email}) was updated from IP {ipAdress}", dbUser.Id, dbUser.Email, Request.HttpContext.Connection.RemoteIpAddress);

		return NoContent();
	}
}

public class ValidationErrorResult(string? title = null, Dictionary<string, string>? errors = null) :
	BadRequestObjectResult(new ProblemDetails
		{
			Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
			Title = title ?? "One or more validation errors occurred.",
			Extensions = errors?.Count > 0 ? new Dictionary<string, object?>
			{
				{ "errors", errors.ToDictionary(x => x.Key, x => new[] { x.Value })	}
			} : []
		})
{
}

public class LoginParameters
{
	[Required, EmailAddress]
	public string? Email { get; set; }
	[Required, MinLength(8, ErrorMessage = "The password must be at least 8 characters")]
	public string? Password { get; set; }
}

public class RegisterUserParameters
{
	[Required]
	public string? Name { get; set; }
	[Required, EmailAddress]
	public string? Email { get; set; }
	[Required, MinLength(8, ErrorMessage = "The password must be at least 8 characters")]
	public string? Password { get; set; }
}

public class ResetPasswordParameters
{
	[Required]
	public string? Token { get; set; }
	[Required, MinLength(8, ErrorMessage = "The password must be at least 8 characters")]
	public string? Password { get; set; }
}

public class UpdateSettingsParameters
{
	[Required]
	public string? Name { get; set; }
	[Required, EmailAddress]
	public string? Email { get; set; }
	public bool Notify { get; set; }
}

public class UpdatePasswordParameters
{
	[Required, MinLength(8, ErrorMessage = "The password must be at least 8 characters")]	public string? Password { get; set; }
}

#nullable disable
public class LoginResponse
{
	public Oauth2AuthResponse Auth { get; set; }
	public User User { get; set; }
}
#nullable enable