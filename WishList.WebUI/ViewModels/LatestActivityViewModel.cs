using System;
using System.Collections.Generic;
using System.Linq;
using WishList.Data;

namespace WishList.WebUI.ViewModels
{
	public class LatestActivityViewModel
	{
		public IList<Wish> Wishes { get; set; }
	}
}
