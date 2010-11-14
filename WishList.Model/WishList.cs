using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishList.Data.Filters;
using WishList.Data.DataAccess;

namespace WishList.Data
{
	public class WishList
	{
		public WishList()
		{
		}

		public WishList( int userId, IList<Wish> wishes )
		{
			UserId = userId;
			Wishes = wishes;
		}

		/// <summary>
		/// Id of the owner of the wish list
		/// </summary>
		public int UserId
		{
			get;
			private set;
		}

		public IList<Wish> Wishes
		{
			get;
			private set;
		}
	}
}
