using WishList.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using System.Linq;
using WishList.Data;
using WishList.Data.Filters;
using WishList.Data.DataAccess;
using System.Web.Routing;
using System.Security.Principal;

namespace WishList.Tests.Controllers
{
	/// <summary>
	///This is a test class for WishControllerTest and is intended
	///to contain all WishControllerTest Unit Tests
	///</summary>
	[TestClass()]
	public class WishControllerTest
	{
		IWishListRepository _repository;
		Services.WishService _wishService;
		Services.UserService _userService;

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestInitialize]
		public void SetUp()
		{
			_repository = new TestWishListRepository();
			_wishService = new Services.WishService( _repository, new TestMailService() );
			_userService = new Services.UserService( _repository );
		}

		[TestMethod]
		public void Can_Create_New_Wish()
		{
			WishController controller = new WishController( _wishService, _userService );
			Wish wish = new Wish
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

			Wish createdWish = _repository.GetWishes().WithName( wish.Name );
			Assert.IsNotNull( createdWish, "Wish was not created" );
		}

		[TestMethod]
		public void Edit_Will_Show_View_With_Wish()
		{
			WishController controller = new WishController( _wishService, _userService );
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
			WishController controller = new WishController( _wishService, _userService );
			//Create a new wish to edit
			Wish wish = _repository.SaveWish( new Wish
				{
					Name = "EditTestWish",
					Description = "Some description",
					Owner = _repository.GetUsers().WithId( 1 ),
					LinkUrl = "http://www.google.com"
				} );
			Wish wishToEdit = new Wish { Name = "Changed name", Description = "Changed description", LinkUrl = "http://www.microsoft.com" };
			IPrincipal user = new GenericPrincipal( new GenericIdentity( "User 1", "Forms" ), null );

			var result = controller.Edit( wish.Id, wishToEdit, user ) as RedirectToRouteResult;

			Assert.IsNotNull( result, "Result was null or not redirecttorouteresult" );
			Assert.AreEqual( "Show", result.RouteValues["action"] );
			Assert.AreEqual( "List", result.RouteValues["controller"] );
			Assert.AreEqual( wish.Owner.Name, result.RouteValues["id"] );

			Wish loadedWish = _repository.GetWishes().WithId( wish.Id );
			Assert.AreEqual( wishToEdit.Name, loadedWish.Name );
			Assert.AreEqual( wishToEdit.Description, loadedWish.Description );
			Assert.AreEqual( wishToEdit.LinkUrl, loadedWish.LinkUrl );
		}
	}
}