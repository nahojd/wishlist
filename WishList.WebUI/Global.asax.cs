using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcContrib.StructureMap;

namespace WishList.WebUI
{
	public class GlobalApplication : System.Web.HttpApplication
	{
		public static void RegisterRoutes( RouteCollection routes )
		{
			routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

			routes.MapRoute(
				"Wish",
				"Wish/{action}/{wishId}",
				new { controller = "Wish", action = "Edit", wishId = -1 }
				);

			routes.MapRoute(
				"ApproveOrDeleteAccount",
				"Account/{action}/{username}/{ticket}",
				new { controller = "Account" }
				);

			routes.MapRoute(
				"Default",                                              // Route name
				"{controller}/{action}/{id}",                           // URL with parameters
				new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
			);
		}

		protected void Application_Start()
		{
			RegisterRoutes( RouteTable.Routes );
			IoC.BootStrapper.ConfigureStructureMap();
			ControllerBuilder.Current.SetControllerFactory( typeof( StructureMapControllerFactory ) );
		}
	}
}