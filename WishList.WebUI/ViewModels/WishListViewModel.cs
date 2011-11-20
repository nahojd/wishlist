using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishList.WebUI.ViewModels
{
	public class WishListViewModel
	{
		public int UserId { get; set; }
		public IList<WishInListViewModel> Wishes { get; set; }
	}
}
