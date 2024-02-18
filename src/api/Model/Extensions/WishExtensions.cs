using Dapper;
using System.Data.Common;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;
public static class WishExtensions
{
	public static async Task<IReadOnlyList<Wish>> GetMyWishes(this DbConnection conn, int userId)
	{
		var wishes = await conn.QueryAsync<Db.Wish>("select * from Wish where OwnerId = @userId", new { userId });

		return wishes.Select(w => Wish.Create(w, null, null)).ToList();
	}

	public static async Task<IReadOnlyList<Wish>> GetWishesForUser(this DbConnection conn, int userId)
	{
		var wishes = await conn.QueryAsync<Db.Wish, Db.User, Db.User?, Model.Wish>(
			@"select * from Wish
			  inner join User Owner on Wish.OwnerId = Owner.Id
			  left outer join User TjingadBy on Wish.TjingadBy = TjingadBy.Id
			  where OwnerId = @userId",
			(w, u, t) => Model.Wish.Create(w, u, t),
			new { userId },
			splitOn: "Id");

		return wishes.ToList();
	}

	public static async Task<Wish?> GetWish(this DbConnection conn, int wishId)
	{
		var wishes = await conn.QueryAsync<Db.Wish, Db.User, Db.User?, Model.Wish>(
			@"select * from Wish
			  inner join User Owner on Wish.OwnerId = Owner.Id
			  left outer join User TjingadBy on Wish.TjingadBy = TjingadBy.Id
			  where Id = @wishId",
			(w, u, t) => Model.Wish.Create(w, u, t),
			new { wishId },
			splitOn: "Id");

		return wishes.SingleOrDefault();
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