using System.Web.Mvc;
using WishList.WebUI.Helpers;
using WishList.Data;
using WishList.Services;
using WishList.Data.DataAccess;
using System.Web.Security;
using WishList.WebUI.ModelBinders;
using System.Security.Principal;
using WishList.Data.Membership;

namespace WishList.WebUI.Components.UserSession.Controllers
{
	public class UserSessionController : Controller
	{
		public void Summary( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentPrincipal )
		{
			if (currentPrincipal != null)
			{
				IWishListRepository repository = new SqlWishListRepository( new MembershipWrapper() );
				UserService service = new UserService( repository );
				User user = service.GetUser( currentPrincipal.Identity.Name );

				View( "Summary", user );
			}
			else
			{
				View( "Summary", null );
			}
		}
	}
}
