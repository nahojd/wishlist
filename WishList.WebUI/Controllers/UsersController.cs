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

namespace WishList.WebUI.Controllers
{
    public partial class UsersController : Controller
	{
        public virtual ActionResult Index()
		{
			// Add action logic here
			throw new NotImplementedException();
		}

		public virtual ActionResult List( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentPrincipal )
		{
			if (currentPrincipal != null)
			{

				SqlWishListRepository repository = new SqlWishListRepository();
				UserService service = new UserService( repository );

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
	}

	public class UsersListData
	{
		public IList<WishList.Data.User> Users { get; set; }

		public string SelectedUsername { get; set; }
	}
}
