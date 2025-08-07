using Dapper;
using System.Data;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;
public static class WishExtensions
{
	public static async Task<IReadOnlyList<Wish>> GetMyWishes(this IDbConnection conn, int userId)
	{
		var wishes = await conn.QueryAsync<Db.Wish>("select * from Wish where OwnerId = @userId", new { userId });

		return wishes.Select(w => Wish.Create(w, null, null)).ToList();
	}

	public static async Task<IReadOnlyList<Wish>> GetWishesForUser(this IDbConnection conn, int userId)
	{
		var wishes = await conn.QueryAsync<Db.Wish, Db.User, Db.User?, Model.Wish>(
			@"select * from Wish
			  inner join User Owner on Wish.OwnerId = Owner.Id
			  left outer join User TjingadBy on Wish.TjingadBy = TjingadBy.Id
			  where OwnerId = @userId",
			(w, u, t) => Wish.Create(w, u, t),
			new { userId },
			splitOn: "Id");

		return [.. wishes];
	}

	public static async Task<Wish?> GetWish(this IDbConnection conn, int wishId)
	{
		var wishes = await conn.QueryAsync<Db.Wish, Db.User, Db.User?, Model.Wish>(
			@"select * from Wish
			  inner join User Owner on Wish.OwnerId = Owner.Id
			  left outer join User TjingadBy on Wish.TjingadBy = TjingadBy.Id
			  where Wish.Id = @wishId",
			(w, u, t) => Wish.Create(w, u, t),
			new { wishId },
			splitOn: "Id");

		return wishes.SingleOrDefault();
	}

	public static async Task<int> AddWish(this IDbConnection conn, int userId, string name, string? description, string? linkUrl)
	{
		var wishId = await conn.QuerySingleAsync<int>(@"insert into Wish (Name, Description, LinkUrl, OwnerId, Created) values (@name, @description, @linkUrl, @userId, @Now);
														select last_insert_id()",
												new { name, description, linkUrl, userId, DateTime.Now });
		return wishId;
	}

	public static Task UpdateWish(this IDbConnection conn, int wishId, string name, string? description, string? linkUrl) =>
		conn.ExecuteAsync("update Wish set Name = @name, Description = @description, LinkUrl = @linkUrl, Updated = @Now where Id = @wishId",
									new { wishId, name, description, linkUrl, DateTime.Now });

	public static Task DeleteWish(this IDbConnection conn, int wishId) =>
		conn.ExecuteAsync("delete from Wish where Id = @wishId", new { wishId });

	public static Task TjingaWish(this IDbConnection conn, int userId, int wishId) =>
		conn.ExecuteAsync("update Wish set TjingadBy = @userId where Id = @wishId", new { userId, wishId });

	public static Task AvtjingaWish(this IDbConnection conn, int wishId) =>
		conn.ExecuteAsync("update Wish set TjingadBy = null where Id = @wishId", new { wishId });
}