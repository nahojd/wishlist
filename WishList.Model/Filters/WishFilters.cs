using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishList.Data.Filters
{
    public static class WishFilters
    {
        /// <summary>
        /// Filters the query by user id
        /// </summary>
        /// <param name="qry">The query.</param>
        /// <param name="userId">The user id to filter by.</param>
        /// <returns>IQueryable of Wish</returns>
        public static IQueryable<Wish> ForUser( this IQueryable<Wish> query, int userId )
        {
            return from wish in query
                   where wish.Owner.Id == userId
                   select wish;
        }

		public static Wish WithName( this IQueryable<Wish> query, string name )
		{
			return (from wish in query
					where wish.Name == name
					select wish).SingleOrDefault<Wish>();
		}

		public static Wish WithId( this IQueryable<Wish> query, int id )
		{
			return (from wish in query
					where wish.Id == id
					select wish).SingleOrDefault<Wish>();
		}

    }
}
