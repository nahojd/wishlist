using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WishList.WebUI.ViewModels
{
	public class WishInListViewModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string LinkUrl { get; set; }

		public int? CalledByUserId { get; set; }
		public string CalledByUserName { get; set; }

		public bool IsCalled { get { return CalledByUserId.HasValue; } }

		
	}
}