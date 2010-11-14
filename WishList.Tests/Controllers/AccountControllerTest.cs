using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.Data.DataAccess;
using WishList.WebUI.Controllers;
using System.Web.Mvc;
using WishList.Tests.Helpers;
using System.Security.Principal;


namespace WishList.Tests.Controllers
{
	/// <summary>
	/// Summary description for AccountControllerTest
	/// </summary>
	[TestClass]
	public class AccountControllerTest
	{
		IWishListRepository _repository;


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
		}

		[TestMethod]
		public void AccountController_Exists_And_Accepts_A_Repository()
		{

			AccountController controller = new AccountController( _repository );
			Assert.IsNotNull( controller );

		}

		[TestMethod]
		public void AccountController_Can_Edit_User()
		{
			var user = _repository.CreateUser( new WishList.Data.User
			{
				Name = "EditTestUser",
				Email = "test@email.com",
				Password = "abc123",
				NotifyOnChange = false
			} );

			var editUser = new WishList.Data.User
			{
				Email = "new@email.com",
				NotifyOnChange = true
			};

			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );
			using (var controller = new AccountController( _repository ))
			{
				ViewResult result = controller.Edit( editUser, currentUser ) as ViewResult;
				Assert.IsNotNull( result, "Did not return a ViewResult" );
				var userData = result.ViewData.Model as AccountController.UserData;
				Assert.IsNotNull( userData, "ViewData was not a userData" );
				Assert.IsNotNull( userData.User, "User was null" );
				Assert.AreEqual( editUser.Email, userData.User.Email, "Email did not change" );
				Assert.AreEqual( editUser.NotifyOnChange, userData.User.NotifyOnChange, "NotifyOnChange did not change" );
			}

		}


		#region Login
		[TestMethod]
		public void AccountController_Index_Redirects_To_Login()
		{

			AccountController controller = new AccountController( _repository );
			ActionResult result = controller.Index();
			Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );

			RedirectToRouteResult renderResult = result as RedirectToRouteResult;
			Assert.AreEqual( "Login", renderResult.RouteValues["action"] );
		}

		[TestMethod]
		public void AccountController_Has_a_Login_Method_That_Renders_The_LoginView()
		{
			AccountController controller = new AccountController( _repository );
			ActionResult result = controller.Login();
			Assert.IsInstanceOfType( result, typeof( ViewResult ) );

			ViewResult renderResult = result as ViewResult;
			Assert.AreEqual( "Login", renderResult.ViewName );
		}

		[TestMethod]
		public void AccountController_Authenticate_Action_Redirects_For_User1_To_TestUrl()
		{
			AccountController controller = new TestableAccountController( _repository );

			//controller.SetFakeControllerContextWithLogin(  );

			ActionResult result = controller.Authenticate( "User 1", "pwd1", "testurl" );

			//a redirect result is a pass - which in this case it should be
			Assert.IsInstanceOfType( result, typeof( RedirectResult ) );

			RedirectResult redirectResult = result as RedirectResult;
			Assert.AreEqual( "testurl", redirectResult.Url );
		}

		[TestMethod]
		public void AccountController_Authenticate_Redirects_To_Login_For_Bad_Login()
		{
			AccountController controller = new AccountController( _repository );

			//controller.SetFakeControllerContextWithLogin( "User 1", "The wrong password", "testurl" );

			ActionResult result = controller.Authenticate( "User 1", "The wrong password", "testurl" );

			Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );
			Assert.IsNotNull( controller.TempData["loginmessage"] );

		}

		#endregion
	}

	internal class TestableAccountController : AccountController
	{
		internal TestableAccountController( IWishListRepository repository )
			: base( repository )
		{
		}

		internal TestableAccountController() : base() { }

		protected override void PerformFormsAuthentication( string userName )
		{
			//Do nothing - we can't test FormsAuthentication anyway.
		}
	}
}
