using Dapper;
using MySqlConnector;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;

public static class WishExtensions
{
	public static async Task<IReadOnlyList<Wish>> GetWishesForUser(this MySqlConnection conn, int userId)
	{
		var wishes = await conn.QueryAsync<Db.Wish>("select * from Wish where OwnerId = @userId", new { userId });

		return wishes.Select(Wish.Create).ToList();
	}
}