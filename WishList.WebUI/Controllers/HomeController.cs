using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WishList.WebUI.Controllers
{
    public partial class HomeController : Controller
	{
        public virtual ActionResult Index()
		{
			ViewData["Title"] = "Önskelistemaskinen 2";
			ViewData["Message"] = "Välkommen till Önskelistemaskinen 2!";

			return View();
		}

        public virtual ActionResult About()
		{
			ViewData["Title"] = "Om önskelistemaskinen";

			return View();
		}
	}
}
