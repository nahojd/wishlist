using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WishList.Services;
using WishList.Data.DataAccess;
using System.Web.Mvc;

namespace WishList.WebUI.Helpers
{
	/// <summary>
	/// Helpers for Views. Can ONLY be used in Views - NEVER from Controllers!
	/// </summary>
	public static class AppHelper
	{
		public static bool UserIsLoggedIn()
		{
			if ( HttpContext.Current == null || HttpContext.Current.User == null )
			{
				return false;
			}

			return HttpContext.Current.User.Identity.IsAuthenticated;
		}

		public static string GetUserName()
		{
			if (HttpContext.Current != null)
			{
				//if not, do we know who they are?
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					return HttpContext.Current.User.Identity.Name;
				}
			}

			return string.Empty;
		}

		public static int GetUserId()
		{
			if (UserIsLoggedIn())
			{
				string userName = GetUserName();
				SqlWishListRepository rep = new SqlWishListRepository();
				UserService svc = new UserService( rep );
				var user = svc.GetUser( userName );
				if (user != null)
				{
					return user.Id;
				}
			}
			return -1;
		}

		public static string ShortUrl( string url )
		{
			try
			{
				Uri uri = new Uri( url );
				return uri.Host;
			}
			catch
			{
				return url.Length > 25 ? url.Substring( 0, 25 ) : url;
			}
		}

		public static bool IsCurrentUserId( int userId )
		{
			return UserIsLoggedIn() && userId == GetUserId();
		}
	}
}
