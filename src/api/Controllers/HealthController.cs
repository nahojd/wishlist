using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WishList.Api.DataAccess;
using WishList.Api.Model.Extensions;

namespace WishList.Api.Controllers;

[ApiController]
[AllowAnonymous]
public class HealthController(IConfiguration config) : Controller
{
	[HttpGet, HttpHead]
	[Route("/health")]
	public async Task<ActionResult> CheckHealth()
	{
		using var conn = DbHelper.OpenConnection(config);
		var dbVersion = await conn.ExecuteScalarAsync<int>("SELECT DbVersion from DbSettings LIMIT 1;");

		if (dbVersion > 0)
			return Json(new { dbVersion });
		return StatusCode(500, "Could not get version from db");
	}
}