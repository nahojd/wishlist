using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WishList.Api.Model;
using WishList.Api.Model.Extensions;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Controllers;

[ApiController]
[Route("wish")]
[Authorize]
public class WishController(IConfiguration config, ILogger<WishController> logger) : ControllerBase
{
	private readonly ILogger<WishController> logger = logger;
	private readonly IConfiguration config = config;

	[HttpGet, Route("list/{userId:int}")]
	public async Task<IEnumerable<Wish>> GetWishesForUser(int userId)
	{
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();

		var wishes = await conn.GetWishesForUser(userId);
		return wishes;
	}

	[HttpGet, Route("list")]
	public async Task<IEnumerable<Wish>> GetMyWishes()
	{
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();

		var userId = User.GetUserId();

		var wishes = await conn.GetWishesForUser(userId!.Value);
		return wishes;
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

//Account (authorized)
//Update password
//Update profile

//Admin
//Pending applications
//Approve application
//Deny application

//Account (authorized)
//Update password
//Update profile

//Admin
//Pending applications
//Approve application
//Deny application