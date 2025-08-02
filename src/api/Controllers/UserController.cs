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
	//Users
	//GET friends
	//GET users
	//Befriend user
	//Unfriend user

	private readonly IConfiguration config = config;

	[HttpGet("list")]
	public async Task<ActionResult<IReadOnlyList<User>>> GetUserList()
	{
		using var conn = DbHelper.OpenConnection(config);

		var users = await conn.GetUserList();
		return Json(users);
	}
}


//Admin
//Pending applications
//Approve application
//Deny application
