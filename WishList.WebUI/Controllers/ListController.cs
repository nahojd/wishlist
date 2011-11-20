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
using WishList.WebUI.ViewModels;

namespace WishList.WebUI.Controllers
{
	[Authorize]
    public class ListController : Controller
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
			var list = _wishService.GetWishList( id );

			var model = GetWishListViewModel( list );

			if (id.Equals( currentUser.Identity.Name, StringComparison.InvariantCultureIgnoreCase ))
			{
				return View( "CurrentUser", model );
			}
			return View( model );
		}

		private WishListViewModel GetWishListViewModel( WishList.Data.WishList list )
		{
			var model = new WishListViewModel {
				UserId = list.UserId,
				Wishes = new List<WishInListViewModel>()
			};

			foreach (var wish in list.Wishes)
			{
				model.Wishes.Add( new WishInListViewModel { 
					Id = wish.Id,
					Name = wish.Name,
					Description = wish.Description,
					LinkUrl = WashUrl( wish.LinkUrl ),
					CalledByUserId = wish.IsCalled ? (int?)wish.CalledByUser.Id : null,
					CalledByUserName = wish.IsCalled ? wish.CalledByUser.Name : null
				} );
			}

			return model;
		}

		private static string WashUrl( string linkUrl )
		{
			if (string.IsNullOrWhiteSpace( linkUrl ))
				return linkUrl;

			if (!linkUrl.StartsWith( "http", StringComparison.InvariantCultureIgnoreCase ))
				return string.Format( "http://{0}", linkUrl );

			return linkUrl;
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

			var model = new LatestActivityViewModel { Wishes = wishes };

			return View( "LatestActivityList", model );
		}

	}
}