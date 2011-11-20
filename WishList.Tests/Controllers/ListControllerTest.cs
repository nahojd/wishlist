using WishList.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using WishList.Data.DataAccess;
using System.Linq;
using System.Security.Principal;
using System.Collections.Generic;
using WishList.Services;
using WishList.WebUI.ViewModels;
using WishList.Data.Filters;

namespace WishList.Tests.Controllers
{


	/// <summary>
	///This is a test class for ListControllerTest and is intended
	///to contain all ListControllerTest Unit Tests
	///</summary>
	[TestClass()]
	public class ListControllerTest
	{
		private ListController controller;
        TestWishListRepository repository;
		WishService wishService;
		UserService userService;

		[TestInitialize]
		public void Setup()
		{
			repository = new TestWishListRepository();
			wishService = new WishService( repository, null );
			userService = new UserService( repository );
			userService.ClearCache();
			controller = new ListController( wishService, userService );
		}

		[TestCleanup]
		public void TearDown()
		{
			if (controller != null)
				controller.Dispose();
		}


		public TestContext TestContext { get; set; }


		[TestMethod]
		public void ListController_Can_Show_WishList()
		{
			var username = "User 1";
			var user = GetPrincipal( username );

			var result = controller.Show( username, user );

			var viewResult = result as ViewResult;
			Assert.IsNotNull( viewResult );
			Assert.IsInstanceOfType( viewResult.ViewData.Model, typeof( WishListViewModel ), "Model was not a WishList" );
			var model = viewResult.ViewData.Model as WishListViewModel;
			Assert.AreEqual( model.UserId, 1, "User id was wrong" );
			Assert.AreEqual( 5, model.Wishes.Count );
		}

		[TestMethod]
		public void Show_WishWithLinkWithoutHttp_AddsHttp()
		{
			repository.Wishes.Where( x => x.Owner.Id == 1 ).First().LinkUrl = "www.test.com";
			var user = GetPrincipal( "Any user" );

			var result = controller.Show( "User 1", user );

			var wish = ((WishListViewModel)((ViewResult)result).ViewData.Model).Wishes.First();

			Assert.AreEqual( "http://www.test.com", wish.LinkUrl );
		}

		[TestMethod]
		[Ignore]
		public void ListController_Can_Show_ShoppingList()
		{
			Assert.Inconclusive();
		}

		[TestMethod]
		public void Can_Get_Latest_Activity_List()
		{
			var user = GetPrincipal( "User 1" );

			var result = controller.LatestActivity( user );

			Assert.IsInstanceOfType( result, typeof( ViewResult ) );
			var viewResult = result as ViewResult;
			Assert.IsInstanceOfType( viewResult.ViewData.Model, typeof( LatestActivityViewModel ), "Model was not an IList of Wishes" );
			Assert.AreEqual( ((LatestActivityViewModel)viewResult.ViewData.Model).Wishes.Count, 10, "Wrong number of wishes" );

		}

		private static IPrincipal GetPrincipal( string username )
		{
			return new GenericPrincipal( new GenericIdentity( username, "Forms" ), null );
		}
	}
}
