using System;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using WishList.WebUI.DependencyResolution;

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
					assemblyScanner.WithDefaultConventions();
					assemblyScanner.With(new ControllerConvention()); //Ensures a new Controller instance is created each time
				} );

			For<WishList.Data.DataAccess.IWishListRepository>().Use( () => new WishList.Data.DataAccess.SqlWishListRepository() );

		}
	}
}
