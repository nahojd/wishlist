using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WishList.Services;
using WishList.Data.DataAccess;
using WishList.WebUI.Helpers;
using WishList.WebUI.ModelBinders;
using System.Security.Principal;

namespace WishList.WebUI.Controllers
{
    public partial class ListController : Controller
	{
		private readonly IWishService _wishService;
		private readonly IUserService _userService;

		public ListController( IWishService wishService, IUserService userService )
		{
			_wishService = wishService;
			_userService = userService;
		}

        public virtual ActionResult Show(string id, [ModelBinder(typeof(IPrincipalModelBinder))] IPrincipal currentUser)
		{
			WishList.Data.WishList list = _wishService.GetWishList( id );

			if (id.Equals( currentUser.Identity.Name, StringComparison.InvariantCultureIgnoreCase ))
			{
				return View( "CurrentUser", list );
			}
			return View( list );
		}

		/// <summary>
		/// Shows the shopping list for the current user
		/// </summary>
		/// <returns></returns>
		public virtual ActionResult ShowShoppingList( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			if (currentUser == null)
			{
				throw new ArgumentNullException( "currentUser" );
			}

			var user = _userService.GetUser( currentUser.Identity.Name );

			var wishes = _wishService.GetShoppingList( user.Id ).OrderBy( x => x.Owner.Name ).ToList();
			return View( wishes );
		}

		public virtual ActionResult LatestActivity([ModelBinder(typeof(IPrincipalModelBinder))] IPrincipal currentUser)
		{
			var user = _userService.GetUser( currentUser.Identity.Name );
			var wishes = _wishService.GetLatestActivityList( user.Id );

			LatestActivityViewModel model = new LatestActivityViewModel { Wishes = wishes };

			return View( "LatestActivityList", model );
		}

	}

	public class LatestActivityViewModel
	{
		public IList<WishList.Data.Wish> Wishes { get; set; }
	}
}
