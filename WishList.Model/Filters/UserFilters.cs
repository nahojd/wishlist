using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishList.Data.Filters
{
	public static class UserFilters
	{
		public static User WithId( this IList<User> query, int userId )
		{
			return (from user in query
					where user.Id == userId
					select user).SingleOrDefault<User>();
		}

		public static User WithName( this IList<User> query, string userName )
		{
			return (from user in query
					where user.Name.ToLower() == userName.ToLower()
					select user).SingleOrDefault<User>();
		}

		public static User WithId( this IQueryable<User> query, int userId )
		{
			return (from user in query
					where user.Id == userId
					select user).SingleOrDefault<User>();
		}

		public static User WithName( this IQueryable<User> query, string userName )
		{
			return (from user in query
					where user.Name.ToUpper() == userName.ToUpper()
					select user).SingleOrDefault<User>();
		}
	}
}
