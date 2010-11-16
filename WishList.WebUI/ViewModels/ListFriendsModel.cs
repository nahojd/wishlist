using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WishList.Data;

namespace WishList.WebUI.ViewModels
{
	public class ListFriendsModel
	{
		public IList<User> Friends { get; set; }
		public string SelectedUsername { get; set; }
	}
}