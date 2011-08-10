using System;
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
					assemblyScanner.WithDefaultConventions();
				} );

			For<WishList.Data.DataAccess.IWishListRepository>().Use( () => new WishList.Data.DataAccess.SqlWishListRepository() );

		}
	}
}
