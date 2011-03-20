using WishList.WebUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using WishList.Data.DataAccess;
using System.Security.Principal;
using System.Collections.Generic;
using WishList.Services;

namespace WishList.Tests.Controllers
{


	/// <summary>
	///This is a test class for ListControllerTest and is intended
	///to contain all ListControllerTest Unit Tests
	///</summary>
	[TestClass()]
	public class ListControllerTest
	{
		IWishListRepository _repository;
		WishService _wishService;
		UserService _userService;

		[TestInitialize]
		public void Setup()
		{
			_repository = new TestWishListRepository();
			_wishService = new WishService( _repository, null );
			_userService = new UserService( _repository );
			_userService.ClearCache();
		}


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

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for Show
		///</summary>
		// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
		// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
		// whether you are testing a page, web service, or a WCF service.
		[TestMethod()]
		//[HostType( "ASP.NET" )]
		//[AspNetDevelopmentServerHost( "D:\\Users\\Johan\\Documents\\Visual Studio 2008\\Projects\\WishList2\\WishList.WebUI", "/" )]
		//[UrlToTest( "http://localhost:49472/" )]
		public void ListController_Can_Show_WishList()
		{
			ListController lc = new ListController( _wishService, _userService );
			string username = "User 1";
			IPrincipal user = new GenericPrincipal( new GenericIdentity( username, "Forms" ), null );

			ActionResult result = lc.Show( username, user );
			Assert.IsInstanceOfType( result, typeof( ViewResult ) );

			ViewResult viewResult = result as ViewResult;

			Assert.IsInstanceOfType( viewResult.ViewData.Model, typeof( WishList.Data.WishList ), "Model was not a WishList" );
			WishList.Data.WishList model = viewResult.ViewData.Model as WishList.Data.WishList;
			Assert.AreEqual( model.UserId, 1, "User id was wrong" );
		}

		//[TestMethod]
		public void ListController_Can_Show_ShoppingList()
		{
			//Cannot currently test this - depends on AppHelper, which depends on HttpContext.
		}

		/// <summary>
		///A test for ListController Constructor
		///</summary>
		// TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
		// http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
		// whether you are testing a page, web service, or a WCF service.
		[TestMethod()]
		//[HostType( "ASP.NET" )]
		//[AspNetDevelopmentServerHost( "D:\\Users\\Johan\\Documents\\Visual Studio 2008\\Projects\\WishList2\\WishList.WebUI", "/" )]
		//[UrlToTest( "http://localhost:49472/" )]
		public void ListControllerConstructorTest()
		{
			ListController lc = new ListController( _wishService, _userService );
			Assert.IsNotNull( lc );
		}

		[TestMethod]
		public void Can_Get_Latest_Activity_List()
		{
			var controller = new ListController( _wishService, _userService );
			IPrincipal user = new GenericPrincipal( new GenericIdentity( "User 1", "Forms" ), null );

            var result = controller.LatestActivity( user );

			Assert.IsInstanceOfType( result, typeof( ViewResult ) );
			ViewResult viewResult = result as ViewResult;
			Assert.IsInstanceOfType( viewResult.ViewData.Model, typeof( WishList.WebUI.Controllers.LatestActivityViewModel ), "Model was not an IList of Wishes" );
			Assert.AreEqual( ((WishList.WebUI.Controllers.LatestActivityViewModel)viewResult.ViewData.Model).Wishes.Count, 10, "Wrong number of wishes" );

		}
	}
}
