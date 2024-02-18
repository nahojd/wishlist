using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using WishList.Api.Model;
using WishList.Api.Model.Extensions;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Controllers;

[ApiController]
[Route("wish")]
[Authorize]
public class WishController(IConfiguration config) : Controller
{
	private readonly IConfiguration config = config;

	[HttpGet("list/{userId:int}")]
	public async Task<ActionResult> GetWishesForUser(int userId)
	{
		if (userId == User.GetUserId())
			return BadRequest(new ProblemDetails { Detail = "Invalid userId!. Just call /wish/list to get your own wishes"});

		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();

		var wishes = await conn.GetWishesForUser(userId);
		return Json(wishes);
	}

	[HttpGet("list")]
	public async Task<ActionResult> GetMyWishes()
	{
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();

		var userId = User.GetUserId();

		var wishes = await conn.GetWishesForUser(userId);
		return Json(wishes);
	}

	[HttpPost("")]
	public async Task<ActionResult> AddWish([FromBody]WishParameters wish)
	{
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();

		var userId = User.GetUserId();

		var wishId = await conn.AddWish(userId, wish.Name!, wish.Description, wish.LinkUrl);

		return Json(await conn.GetWish(wishId));
	}

	[HttpDelete("{wishId:int}")]
	public async Task<ActionResult> DeleteWish(int wishId)
	{
		using var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();

		var userId = User.GetUserId();
		var wish = await conn.GetWish(wishId);
		if (wish is null)
			return NotFound();

		if (wish.OwnerId != userId)
			return BadRequest(new ProblemDetails { Detail = "That's not your wish to delete!"});

		await conn.DeleteWish(wishId);
		return NoContent();

	}

	//Needed operations
	//CRUD for wishes
	//Call dibs on a wish
	//Remove dibs from a wish
	//My dibs
	//Latest wishes
}

public class WishParameters
{
	[Required]
	public string? Name { get; set; }
	public string? Description { get; set; }
	public string? LinkUrl { get; set; }
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