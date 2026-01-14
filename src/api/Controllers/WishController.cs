using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WishList.Api.DataAccess;
using WishList.Api.Model;
using WishList.Api.Model.Extensions;
using WishList.Api.Services;

namespace WishList.Api.Controllers;

[ApiController]
[Route("wish")]
[Produces("application/json")]
[Authorize]
public class WishController(IConfiguration config, IMessageService messageService) : Controller
{
	private readonly IConfiguration config = config;

	[HttpGet("list/{userId:int}")]
	public async Task<ActionResult<IReadOnlyList<Wish>>> GetWishesForUser(int userId)
	{
		if (userId == User.GetUserId())
			return await GetMyWishes();

		using var conn = DbHelper.OpenConnection(config);

		var wishes = await conn.GetWishesForUser(userId);
		return Json(wishes);
	}

	[HttpGet("list")]
	public async Task<ActionResult<IReadOnlyList<Wish>>> GetMyWishes()
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();

		var wishes = await conn.GetMyWishes(userId);
		return Json(wishes);
	}

	[HttpGet("shoppinglist")]
	public async Task<ActionResult<IReadOnlyList<Wish>>> GetShoppingList()
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();

		var wishes = await conn.GetShoppinglist(userId);
		return Json(wishes);
	}

	[HttpPost("")]
	public async Task<ActionResult<Wish>> AddWish([FromBody]WishParameters wish)
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();

		var wishId = await conn.AddWish(userId, wish.Name, wish.Description, wish.LinkUrl);

		return Json(await conn.GetWish(wishId));
	}

	[HttpPatch("{wishId:int}")]
	public async Task<ActionResult<Wish>> UpdateWish(int wishId, [FromBody]WishParameters updatedWish)
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();
		var wish = await conn.GetWish(wishId);
		if (wish is null)
			return NotFound();

		if (wish.Owner?.Id != userId)
			return StatusCode((int)HttpStatusCode.Forbidden, new ProblemDetails { Detail = "Du kan inte uppdatera någon annans önskning!"});

		await conn.UpdateWish(wishId, updatedWish.Name, updatedWish.Description, updatedWish.LinkUrl);

		if (wish.TjingadBy is not null)
			await messageService.NotifyWishChanged(wish);

		return Json(await conn.GetWish(wishId));
	}

	[HttpDelete("{wishId:int}")]
	public async Task<ActionResult> DeleteWish(int wishId)
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();
		var wish = await conn.GetWish(wishId);
		if (wish is null)
			return NotFound();

		if (wish.Owner?.Id != userId)
			return StatusCode((int)HttpStatusCode.Forbidden, new ProblemDetails { Detail = "Du kan inte radera någon annans önskning!"});

		await conn.DeleteWish(wishId);

		if (wish.TjingadBy is not null)
			await messageService.NotifyWishDeleted(wish);

		return NoContent();

	}

	[HttpPost("{wishId:int}/tjinga")]
	public async Task<ActionResult> TjingaWish(int wishId)
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();
		var wish = await conn.GetWish(wishId);
		if (wish is null)
			return NotFound();

		if (wish.Owner?.Id == userId)
			return StatusCode((int)HttpStatusCode.Forbidden, new ProblemDetails { Detail = "Du kan inte tjinga din egen önskning!"});
		if (wish.TjingadBy != null)
			return BadRequest(new ProblemDetails { Detail = "Önskningen är redan tjingad!" });

		await conn.TjingaWish(userId, wishId);

		return Json(await conn.GetWish(wishId));
	}

	[HttpPost("{wishId:int}/avtjinga")]
	public async Task<ActionResult> AvtjingaWish(int wishId)
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();
		var wish = await conn.GetWish(wishId);
		if (wish is null)
			return NotFound();

		if (wish.Owner?.Id == userId)
			return StatusCode((int)HttpStatusCode.Forbidden, new ProblemDetails { Detail = "Du kan inte tjnga din egen önskning!"});
		if (wish.TjingadBy?.Id != userId)
			return BadRequest(new ProblemDetails { Detail = "Du har inte tjingat önskningen." });

		await conn.AvtjingaWish(wishId);

		return Json(await conn.GetWish(wishId));
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
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? LinkUrl { get; set; }
}


//Admin
//Pending applications
//Approve application
//Deny application



//Admin
//Pending applications
//Approve application
//Deny application