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
					assemblyScanner.With<DefaultConventionScanner>();
				} );

			ForRequestedType<WishList.Data.DataAccess.IWishListRepository>().TheDefault.Is.ConstructedBy( () => new WishList.Data.DataAccess.SqlWishListRepository() );

		}
	}
}
