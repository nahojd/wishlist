using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using WishList.Api.Model;

namespace WishList.Api.Controllers;

[ApiController]
[Route("wish")]
public class WishController : ControllerBase
{
	private readonly ILogger<WishController> logger;
	private readonly IConfiguration config;

	public WishController(IConfiguration config, ILogger<WishController> logger)
	{
		this.logger = logger;
		this.config = config;
	}

	[HttpGet, Route("list/{userId:int}")]
	public async Task<IEnumerable<Wish>> GetWishes(int userId)
	{
		using var conn = new SqlConnection(config.GetConnectionString("WishDB"));
		conn.Open();

		var wishes = await conn.QueryAsync<Wish>("select * from Wish where OwnerId = @userId", new { userId });

		return wishes;
	}

	//Needed operations
	//CRUD for wishes
	//Call dibs on a wish
	//Remove dibs from a wish
	//My dibs
	//Latest wishes

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

	//Account (authorized)
	//Update password
	//Update profile

	//Admin
	//Pending applications
	//Approve application
	//Deny application
}
