using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WishList.Data.DataAccess;
using WishList.Services;
using WishList.WebUI.Helpers;
using System.Web.Routing;
using WishList.WebUI.ModelBinders;
using System.Security.Principal;
using WishList.WebUI.ViewModels;

namespace WishList.WebUI.Controllers
{
	public partial class UsersController : Controller
	{
		private IUserService service;

		public UsersController() : this( new UserService() ) { }

		public UsersController( IUserService service )
		{
			this.service = service;
		}

		public virtual ActionResult Index()
		{
			// Add action logic here
			throw new NotImplementedException();
		}

		public virtual ActionResult List( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentPrincipal )
		{
			if (currentPrincipal != null)
			{
				UsersListData data = new UsersListData { Users = service.GetUsers() };

				//HACK: Is this a call to the Show action in the List controller? Route should not be hard coded!!
				if (Request.Path.ToLower().Contains( "/list/show/" ))
				{
					data.SelectedUsername = Request.Path.Remove( 0, 11 );
				}

				return View( data );
			}
			return null;
		}

		[Authorize]
		public ActionResult ListFriends( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentUser )
		{
			var user = service.GetUser( currentUser.Identity.Name );
			var userList = service.GetFriends( user );
			userList.Insert( 0, user );
			var model = new ListFriendsModel { Friends = userList };


			string id = RouteData.Values["id"] as string;
			if (!string.IsNullOrWhiteSpace( id ))
			{
				model.SelectedUsername = id;
			}

			return View( model );
		}
	}

	public class UsersListData
	{
		public IList<WishList.Data.User> Users { get; set; }

		public string SelectedUsername { get; set; }
	}
}
