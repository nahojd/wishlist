﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace WishList.WebUI.ModelBinders
{
	public class IPrincipalModelBinder : IModelBinder
	{
		public object BindModel( ControllerContext controllerContext, ModelBindingContext bindingContext )
		{
			if (controllerContext == null)
			{
				throw new ArgumentNullException( "controllerContext" );
			}
			if (bindingContext == null)
			{
				throw new ArgumentNullException( "bindingContext" );
			}

			IPrincipal p = controllerContext.HttpContext.User;
			return p;
		}

	}
}
