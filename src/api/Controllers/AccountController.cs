using System.ComponentModel.DataAnnotations;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WishList.Api.Security;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Controllers;



//Account (unauthorized)
//Register account
//Forgot password
//Login
//Logout (doesn't need to be authorized)
[ApiController]
[Route("account")]
public class AccountController(IConfiguration config, ILogger<AccountController> logger) : Controller
{
	private readonly IConfiguration config = config;
	private readonly ILogger<AccountController> logger = logger;

	[HttpPost("register")]
	[AllowAnonymous]
	public async Task<ActionResult> Register([FromBody]RegisterUserParameters parameters)
	{
		//TODO: Kolla att användaren inte redan är registrerad
		//TODO: Validera epost och lösenord (på minsta längd)

		var passwordHasher = new PasswordHasher<string>();
		var hash = passwordHasher.HashPassword(parameters.Email!, parameters.Password!);

		//Verify?
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, hash, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			throw new InvalidOperationException("Password verification failed!");


		//Spara till databas
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();
		await conn.ExecuteAsync("insert into User (Name, Email, Password) values (@Name, @Email, @hash)", new { parameters.Name, parameters.Email, hash });
		logger.LogInformation("Registered user {Name} with email {Email}", parameters.Name, parameters.Email);

		return Ok();
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult> Login([FromBody]LoginParameters parameters)
	{
		//Hämta användaren
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();
		var dbUser = await conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where Email = @Email", new { parameters.Email });
		if (dbUser is null || !dbUser.Verified)
			return BadRequest(new ProblemDetails { Detail = "Invalid email or password"});

		//Validera lösenordet
		var passwordHasher = new PasswordHasher<string>();
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, dbUser.Password!, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			return BadRequest(new ProblemDetails { Detail = "Invalid email or password"});

		//Generera och returnera JWT-token
		var expires = DateTime.Now.AddSeconds(1800);
		var (_, accessToken) = JwtHelper.GenerateToken(config, dbUser.Id, expires);

		return Json(new {
			Auth = new Oauth2AuthResponse(accessToken, 1800),
			User = Model.User.Create(dbUser)
		});
	}

	[HttpPost("refreshlogin")]
	public ActionResult RefreshLogin()
	{
		var userId = User.GetUserId();
		if (userId is null)
			return Unauthorized();

		var expires = DateTime.Now.AddSeconds(1800);
		var (_, accessToken) = JwtHelper.GenerateToken(config, userId.Value, expires);
		return Json(new Oauth2AuthResponse(accessToken, 1800));
	}
}

public class LoginParameters
{
	[Required]
	public string? Email { get; set; }
	[Required]
	public string? Password { get; set; }
}

public class RegisterUserParameters
{
	[Required]
	public string? Name { get; set; }
	[Required]
	public string? Email { get; set; }
	[Required]
	public string? Password { get; set; }
}