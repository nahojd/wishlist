using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace WishList.WebUI.IoC
{
	internal class WishListRegistry : Registry
	{
		internal WishListRegistry()
		{
			Scan( assemblyScanner =>
				{
					assemblyScanner.TheCallingAssembly();
					assemblyScanner.Assembly( "WishList.Data" );
					assemblyScanner.Assembly( "WishList.Services" );
					assemblyScanner.With<DefaultConventionScanner>();
				} );

			ForRequestedType<WishList.Data.DataAccess.IWishListRepository>().TheDefaultIsConcreteType<WishList.Data.DataAccess.SqlWishListRepository>();
			
			//För den inbyggda account-controllern. Kan tas bort om den inte längre används.
			//ForRequestedType<Kurshantering.WebUI.Controllers.IFormsAuthentication>().TheDefaultIsConcreteType<Kurshantering.WebUI.Controllers.FormsAuthenticationService>();
			//ForRequestedType<Kurshantering.WebUI.Controllers.IMembershipService>().TheDefaultIsConcreteType<Kurshantering.WebUI.Controllers.AccountMembershipService>();
			//ForRequestedType<System.Web.Security.MembershipProvider>().TheDefault.IsThis( System.Web.Security.Membership.Provider );
		}
	}
}
