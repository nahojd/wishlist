using System.Web.Mvc;
using WishList.Data;
using WishList.Services;
using WishList.Data.DataAccess;
using WishList.WebUI.ModelBinders;
using System.Security.Principal;

namespace WishList.WebUI.Components.UserSession.Controllers
{
	public class UserSessionController : Controller
	{
		public void Summary( [ModelBinder( typeof( IPrincipalModelBinder ) )] IPrincipal currentPrincipal )
		{
			if (currentPrincipal != null)
			{
				IWishListRepository repository = new SqlWishListRepository();
				UserService service = new UserService( repository, new MailService() );
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
