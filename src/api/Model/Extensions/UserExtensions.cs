using Dapper;
using System.Data;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;

public static class UserExtensions
{
	public static async Task<IReadOnlyList<User>> GetUserList(this IDbConnection conn)
	{
		var users = await conn.QueryAsync<Db.User>("select * from User where Verified = 1 order by Name");

		return [.. users.Select(x => new User { Id = x.Id, Name = x.Name })];
	}

	public static async Task<IReadOnlyList<int>> GetFriends(this IDbConnection conn, int userId)
	{
		var users = await conn.QueryAsync<int>("select FriendId from Friend where UserId = @userId", new { userId });

		return [.. users];
	}

	public static async Task<IReadOnlyList<Db.User>> GetAllDbUsers(this IDbConnection conn)
		=> [.. await conn.QueryAsync<Db.User>("select * from User") ];

	public static Task<Db.User?> GetDbUserByEmail(this IDbConnection conn, string? email)
		=> conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where Email = @email", new { email });

	public static Task<Db.User?> GetDbUserByResetPwdToken(this IDbConnection conn, string? token)
		=> conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where PwdResetToken = @token", new { token });

	public static Task<Db.User?> GetDbUserById(this IDbConnection conn, int id)
		=> conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where Id = @id", new { id });
}
