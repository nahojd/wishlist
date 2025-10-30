using Dapper;
using System.Data;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;

public static class UserExtensions
{
	public static async Task<IReadOnlyList<User>> GetUserList(this IDbConnection conn)
	{
		var users = await conn.QueryAsync<Db.User>("select * from User where Verified = 1 order by Name");

		return [.. users.Select(User.Create)];
	}

	public static Task<Db.User?> GetDbUserByEmail(this IDbConnection conn, string? email)
		=> conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where Email = @email", new { email });

	public static Task<Db.User?> GetDbUserByResetPwdToken(this IDbConnection conn, string? token)
		=> conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where PwdResetToken = @token", new { token });

	public static Task<Db.User?> GetDbUserById(this IDbConnection conn, int id)
		=> conn.QuerySingleOrDefaultAsync<Db.User>("select * from User where Id = @id", new { id });
}
