using System.Data;
using MySqlConnector;

namespace WishList.Api.DataAccess;

public static class DbHelper {

	public static IDbConnection OpenConnection(IConfiguration config)
	{
		var conn = new MySqlConnection(config.WishListConnectionString());
		conn.Open();
		return conn;
	}
}