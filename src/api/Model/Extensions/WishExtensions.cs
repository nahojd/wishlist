using Dapper;
using System.Data.Common;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;

public static class WishExtensions
{
	public static async Task<IReadOnlyList<Wish>> GetWishesForUser(this DbConnection conn, int userId)
	{
		var wishes = await conn.QueryAsync<Db.Wish>("select * from Wish where OwnerId = @userId", new { userId });

		return wishes.Select(Wish.Create).ToList();
	}

	public static async Task<Wish?> GetWish(this DbConnection conn, int wishId)
	{
		var dbWish = await conn.QuerySingleOrDefaultAsync<Db.Wish>("select * from Wish where Id = @wishId", new { wishId });
		if (dbWish is null)
			return null;
		return Wish.Create(dbWish);
	}

	public static async Task<int> AddWish(this DbConnection conn, int userId, string name, string? description, string? linkUrl)
	{
		var wishId = await conn.QuerySingleAsync<int>(@"insert into Wish (Name, Description, LinkUrl, OwnerId) values (@name, @description, @linkUrl, @userId);
														select last_insert_id()",
												new { name, description, linkUrl, userId });
		return wishId;
	}

	public static async Task DeleteWish(this DbConnection conn, int wishId)
	{
		await conn.ExecuteAsync("delete from Wish where Id = @wishId", new { wishId });
	}
}