using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WishList.Api.Model;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Controllers;

[ApiController]
[Route("wish")]
public class WishController(IConfiguration config, ILogger<WishController> logger) : ControllerBase
{
	private readonly ILogger<WishController> logger = logger;
	private readonly IConfiguration config = config;

	[HttpGet, Route("list/{userId:int}")]
	public async Task<IEnumerable<Wish>> GetWishes(int userId)
	{
		using var conn = new MySqlConnection(config.GetConnectionString("WishList"));
		conn.Open();

		var wishes = await conn.QueryAsync<Db.Wish>("select * from Wish"); // where OwnerId = @userId", new { userId });

		return wishes.Select(Wish.Create);
	}

	//Needed operations
	//CRUD for wishes
	//Call dibs on a wish
	//Remove dibs from a wish
	//My dibs
	//Latest wishes


}

//Users
//GET friends
//GET users
//Befriend user
//Unfriend user

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
		using var conn = new MySqlConnection(config.GetConnectionString("WishList"));
		conn.Open();
		await conn.ExecuteAsync("insert into User (Name, Email, Password) values (@Name, @Email, @hash)", new { parameters.Name, parameters.Email, hash });
		logger.LogInformation("Registered user {Name} with email {Email}", parameters.Name, parameters.Email);

		return Ok();
	}

	[HttpPost("login")]
	[AllowAnonymous]
	public async Task<ActionResult> Register([FromBody]LoginParameters parameters)
	{
		//Hämta användaren
		using var conn = new MySqlConnection(config.GetConnectionString("WishList"));
		conn.Open();
		var dbUser = await conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where Email = @Email", new { parameters.Email });
		if (dbUser is null)
			return BadRequest(new ProblemDetails { Detail = "Invalid email or password"});

		//TODO: Ska inte kunna logga in användare som inte är verifierad heller

		//Validera lösenordet
		var passwordHasher = new PasswordHasher<string>();
		var verifyResult = passwordHasher.VerifyHashedPassword(parameters.Email!, dbUser.Password!, parameters.Password!);
		if (verifyResult != PasswordVerificationResult.Success)
			return BadRequest(new ProblemDetails { Detail = "Invalid email or password"});

		//TODO: Generera och returnera JWT-token

		return Json(Model.User.Create(dbUser));
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

//Account (authorized)
//Update password
//Update profile

//Admin
//Pending applications
//Approve application
//Deny application