using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StructureMap;

namespace WishList.WebUI.IoC
{
	internal static class BootStrapper
	{
		internal static void ConfigureStructureMap()
		{
			ObjectFactory.Initialize( x => x.AddRegistry( new WishListRegistry() ) );
		}
	}
}