using WishList.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using System.Linq;
using WishList.Data;
using WishList.Data.Filters;
using WishList.Data.DataAccess;
using System.Security.Principal;
using WishList.Services;

namespace WishList.Tests.Controllers
{
	/// <summary>
	///This is a test class for WishControllerTest and is intended
	///to contain all WishControllerTest Unit Tests
	///</summary>
	[TestClass()]
	public class WishControllerTest
	{
		IWishListRepository repository;
		WishService wishService;
		UserService userService;
		WishController controller;

		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void SetUp()
		{
			repository = new TestWishListRepository();
			wishService = new WishService( repository, new TestMailService() );
			userService = new UserService( repository );
			controller = new WishController( wishService, userService );
		}

		[TestCleanup]
		public void TearDown()
		{
			if (controller != null)
				controller.Dispose();
		}

		[TestMethod]
		public void Can_Create_New_Wish()
		{
			var wish = new Wish
										{
											Name = "A brand new wish",
											Description = "This is a brand new wish",
											LinkUrl = "http://www.google.com"
										};

			IPrincipal user = new GenericPrincipal( new GenericIdentity( "User 1", "Forms" ), null );

			var result = controller.New( wish, user ) as RedirectToRouteResult;

			Assert.IsNotNull( result, "Result was null or not redirecttorouteresult" );
			Assert.AreEqual( "Show", result.RouteValues["action"] );
			Assert.AreEqual( "List", result.RouteValues["controller"] );

			Wish createdWish = repository.GetWishes().WithName( wish.Name );
			Assert.IsNotNull( createdWish, "Wish was not created" );
		}

		[TestMethod]
		public void Edit_Will_Show_View_With_Wish()
		{
			IPrincipal user = new GenericPrincipal( new GenericIdentity( "User 1", "Forms" ), null );

			var result = controller.Edit( 1, user ) as ViewResult;

			Assert.IsNotNull( result );
			Wish wish = result.ViewData.Model as Wish;
			Assert.IsNotNull( wish, "ViewData.Model was not a wish" );
			Assert.AreEqual( 1, wish.Id, "Wrong wish in model" );
		}

		[TestMethod]
		public void Can_Edit_Wish()
		{
			//Create a new wish to edit
			Wish wish = repository.SaveWish( new Wish
				{
					Name = "EditTestWish",
					Description = "Some description",
					Owner = repository.GetUsers().WithId( 1 ),
					LinkUrl = "http://www.google.com"
				} );
			Wish wishToEdit = new Wish { Name = "Changed name", Description = "Changed description", LinkUrl = "http://www.microsoft.com" };
			IPrincipal user = new GenericPrincipal( new GenericIdentity( "User 1", "Forms" ), null );

			var result = controller.Edit( wish.Id, wishToEdit, user ) as RedirectToRouteResult;

			Assert.IsNotNull( result, "Result was null or not redirecttorouteresult" );
			Assert.AreEqual( "Show", result.RouteValues["action"] );
			Assert.AreEqual( "List", result.RouteValues["controller"] );
			Assert.AreEqual( wish.Owner.Name, result.RouteValues["id"] );

			Wish loadedWish = repository.GetWishes().WithId( wish.Id );
			Assert.AreEqual( wishToEdit.Name, loadedWish.Name );
			Assert.AreEqual( wishToEdit.Description, loadedWish.Description );
			Assert.AreEqual( wishToEdit.LinkUrl, loadedWish.LinkUrl );
		}
	}
}