using System.Data;
using Dapper;
using MySqlConnector;
using WishList.Api.DataAccess.Entities;

namespace WishList.Api.DataAccess;

public static class DbExtensions {

	public static IDbConnection OpenConnection(IConfiguration config)
	{
		var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();
		return conn;
	}

	public static Task<User?> GetUserByEmail(this IDbConnection conn, string? email)
		=> conn.QuerySingleOrDefaultAsync<User>("select * from User where Email = @email", new { email });

	public static Task<User?> GetUserByResetPwdToken(this IDbConnection conn, string? token)
		=> conn.QuerySingleOrDefaultAsync<User>("select * from User where PwdResetToken = @token", new { token });
}