using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using WishList.Api.DataAccess;
using WishList.Api.Model;
using WishList.Api.Model.Extensions;

namespace WishList.Api.Controllers;

[ApiController]
[Route("user")]
[Produces("application/json")]
[Authorize]
public class UserController(IConfiguration config) : Controller
{

	[HttpGet("list")]
	public async Task<ActionResult<IReadOnlyList<User>>> GetUserList()
	{
		using var conn = DbHelper.OpenConnection(config);

		//TODO: Gör detta i bara ett databasanrop, det blir lite snyggare (fast det spelar egentligen ingen roll)
		var users = await conn.GetUserList();
		var friends = await conn.GetFriends(User.GetUserId());

		foreach (var user in users)
			user.IsFriend = friends.Contains(user.Id);

		return Json(users);
	}

	[HttpPost("{friendId}/toggleFriend")]
	public async Task<ActionResult> ToggleFriendStatus(int friendId)
	{
		using var conn = DbHelper.OpenConnection(config);

		var userId = User.GetUserId();
		var friends = await conn.GetFriends(userId);
		if (friends.Contains(friendId))
			await conn.ExecuteAsync("delete from Friend where UserId = @userId and FriendId = @friendId", new { userId, friendId });
		else
			await conn.ExecuteAsync("insert into Friend (UserId, FriendId) values (@userId, @friendId)", new { userId, friendId });

		return Ok();
	}
}


//Admin
//Pending applications
//Approve application
//Deny application
