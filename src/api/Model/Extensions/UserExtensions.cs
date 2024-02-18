using Dapper;
using System.Data.Common;
using Db = WishList.Api.DataAccess.Entities;

namespace WishList.Api.Model.Extensions;

public static class UserExtensions
{
	public static async Task<IReadOnlyList<User>> GetUserList(this DbConnection conn)
	{
		var users = await conn.QueryAsync<Db.User>("select * from User where Verified = 1 order by Name");

		return users.Select(User.Create).ToList();
	}
}
