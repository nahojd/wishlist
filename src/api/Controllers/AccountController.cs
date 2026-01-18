using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WishList.Api.DataAccess;
using WishList.Api.Model;
using WishList.Api.Model.Extensions;
using WishList.Api.Security;
using WishList.Api.Services;

namespace WishList.Api.Controllers;

[ApiController]
[Route("account")]
[Produces("application/json")]
public class AccountController(IConfiguration config, IMessageService messageService, ILogger<AccountController> logger) : Controller
{
	private readonly IConfiguration config = config;
	private readonly ILogger<AccountController> logger = logger;

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<ActionResult> Register([FromBody] RegisterUserParameters parameters)
	{
		using var conn = DbHelper.OpenConnection(config);

		//Kolla att användaren inte redan är registrerad
		var users = await conn.GetAllDbUsers();
		if (users.Any(x => x.Email?.Equals(parameters.Email, StringComparison.InvariantCultureIgnoreCase) == true))
			return Conflict(new ProblemDetails { Detail = "En användare med den epostadressen finns redan." });
		if (users.Any(x => x.Name?.Equals(parameters.Name, StringComparison.InvariantCultureIgnoreCase) == true))
			return Conflict(new ProblemDetails { Detail = "En användare med det namnet finns redan." });

		var passwordHasher = new PasswordHasher<string>();
		var hash = passwordHasher.HashPassword(parameters.Email!, parameters.Password!);

		//Verify?
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, hash, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			throw new InvalidOperationException("Password verification failed!");


		//Spara till databas
		await conn.ExecuteAsync("insert into User (Name, Email, Password) values (@Name, @Email, @hash)",
			new { parameters.Name, Email = parameters.Email!.ToLowerInvariant(), hash });
		logger.LogInformation("Registered user {Name} with email {Email}", parameters.Name, parameters.Email);

		await messageService.SendNewRegistrationMessage(parameters.Name!, parameters.Email!, parameters.Message);

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
			return BadRequest(new ProblemDetails { Detail = "Felaktig e-postadress eller lösenord" });

		//Validera lösenordet
		var passwordHasher = new PasswordHasher<string>();
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, dbUser.Password!, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			return BadRequest(new ProblemDetails { Detail = "Felaktig e-postadress eller lösenord" });

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
			return BadRequest(new ProblemDetails { Detail = "Ingen användare med denna epostadress finns registrerad." });

		//Create token by hashing a guid and onverting to base64 string
		var guid = Guid.NewGuid();
		var token = Convert.ToBase64String(SHA1.HashData(Encoding.UTF8.GetBytes(guid.ToString())));
		token = token.Replace('+', '-'); //Pga url...
		await conn.ExecuteAsync("update User set PwdResetToken = @token, PwdResetExpires = @expires where Id = @Id",
								new { token, expires = DateTime.Now.AddHours(24), dbUser.Id });

		await messageService.SendForgotPasswordMessage(dbUser.Name!, dbUser.Email!, token);

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
			return BadRequest(new ProblemDetails { Detail = "Felaktig token" });

		if (!parameters.Password.IsValidPassword())
			return BadRequest(new ProblemDetails { Detail = "Ogiltigt lösenord. Lösenordet måste vara minst 8 tecken." });

		//Uppdatera lösenord
		var passwordHasher = new PasswordHasher<string>();
		var hash = passwordHasher.HashPassword(dbUser.Email!, parameters.Password!);
		await conn.ExecuteAsync("update User set Password = @hash, PwdResetToken = null, PwdResetExpires = null where Id = @Id", new { dbUser.Id, hash });

		logger.LogInformation("Password for user {userId} ({email}) was reset from IP {ipAdress}", dbUser.Id, dbUser.Email, Request.HttpContext.Connection.RemoteIpAddress);

		return Ok();
	}

	[HttpPost("settings")]
	[Authorize]
	public async Task<ActionResult<User>> UpdateUserSettings([FromBody] UpdateSettingsParameters parameters)
	{
		var userId = User.GetUserId();
		using var conn = DbHelper.OpenConnection(config);

		var dbUser = await conn.GetDbUserById(userId);
		if (dbUser is null)
			return Unauthorized();

		//Kolla att användarnamnet eller epostadressen inte redan är upptaget av någon annan
		var users = await conn.GetAllDbUsers();
		var errors = new Dictionary<string, string>();
		if (users.Any(x => x.Id != userId && x.Name?.Equals(parameters.Name, StringComparison.InvariantCultureIgnoreCase) == true))
			errors["Name"] = "Det finns redan en användare med det namnet.";
		if (users.Any(x => x.Id != userId && x.Email?.Equals(parameters.Email, StringComparison.InvariantCultureIgnoreCase) == true))
			errors["Email"] = "Det finns redan en användare med den epostadressen.";
		if (errors.Count > 0)
			return new ValidationErrorResult(errors: errors);

		await conn.ExecuteAsync("update User set Name = @Name, Email = @Email, Notify = @Notify where Id = @userId",
			new { userId, parameters.Name, Email = parameters.Email?.ToLowerInvariant(), parameters.Notify });

		dbUser = await conn.GetDbUserById(userId);

		return Json(Model.User.Create(dbUser!));
	}

	[HttpPost("password")]
	[Authorize]
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
	[Required, MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken långt")]
	public string? Password { get; set; }
}

public class RegisterUserParameters
{
	[Required]
	public string? Name { get; set; }
	[Required, EmailAddress]
	public string? Email { get; set; }
	[Required, MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken långt")]
	public string? Password { get; set; }
	public string? Message { get; set; }
}

public class ResetPasswordParameters
{
	[Required]
	public string? Token { get; set; }
	[Required, MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken långt.")]
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
	[Required, MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken långt.")]
	public string? Password { get; set; }
}

#nullable disable
public class LoginResponse
{
	public Oauth2AuthResponse Auth { get; set; }
	public User User { get; set; }
}
#nullable enable