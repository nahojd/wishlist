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
using WishList.Services;
using Moq;
using WishList.Data;


namespace WishList.Tests.Controllers
{
	/// <summary>
	/// Summary description for AccountControllerTest
	/// </summary>
	[TestClass]
	public class AccountControllerTest
	{
		Mock<IUserService> _service;
		private AccountController controller;

		[TestInitialize]
		public void SetUp()
		{
			_service = new Mock<IUserService>();
			controller = new AccountController( _service.Object );
		}

		[TestMethod]
		public void AccountController_Can_Edit_User()
		{
			var user = new User
									{
										Name = "EditTestUser",
										Email = "test@email.com",
										Password = "abc123",
										NotifyOnChange = false
									};
			_service.Setup( x => x.GetUser( It.IsAny<string>() ) ).Returns( user );
			_service.Setup( x => x.UpdateUser( It.IsAny<User>() ) ).Returns( ( User x ) => x );
			_service.Setup( x => x.GetFriends( It.IsAny<User>() ) ).Returns( new List<User>() );
			_service.Setup( x => x.GetUsers() ).Returns( new List<User>() );

			var editUser = new User
			{
				Email = "new@email.com",
				NotifyOnChange = true
			};
			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );

			var result = controller.Edit( editUser, currentUser ) as ViewResult;

			Assert.IsNotNull( result, "Did not return a ViewResult" );
			var userData = result.ViewData.Model as AccountController.UserData;
			Assert.IsNotNull( userData, "ViewData was not a userData" );
			Assert.IsNotNull( userData.User, "User was null" );
			Assert.AreEqual( editUser.Email, userData.User.Email, "Email did not change" );
			Assert.AreEqual( editUser.NotifyOnChange, userData.User.NotifyOnChange, "NotifyOnChange did not change" );
		}

		[TestMethod]
		public void PostToEdit_ShouldReturnModelWithDictionaryOfFriends()
		{
			User user = new User { Name = "friendstest" };
			_service.Setup( x => x.GetUser( It.IsAny<string>() ) ).Returns( user );
			_service.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Name = "User1" }, new User { Name = "User2" }, new User { Name = "User3" } } );
			_service.Setup( x => x.GetFriends( user ) ).Returns( new List<User> { new User { Name = "User1" }, new User { Name = "User3" } } );
			_service.Setup( x => x.UpdateUser( It.IsAny<User>() ) ).Returns( ( User x ) => x );

			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );

			var result = controller.Edit( user, currentUser ) as ViewResult;
			var model = result.ViewData.Model as AccountController.UserData;

			Assert.IsNotNull( model.Friends );
			Assert.AreEqual( model.Friends.Count, 3 );
			Assert.IsTrue( model.Friends["User1"] );
			Assert.IsFalse( model.Friends["User2"] );
			Assert.IsTrue( model.Friends["User3"] );
		}

		[TestMethod]
		public void Edit_ShouldReturnModelWithDictionaryOfFriends()
		{
			User user = new User { Name = "friendstest" };
			_service.Setup( x => x.GetUser( It.IsAny<string>() ) ).Returns( user );
			_service.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Name = "User1" }, new User { Name = "User2" }, new User { Name = "User3" } } );
			_service.Setup( x => x.GetFriends( user ) ).Returns( new List<User> { new User { Name = "User1" }, new User { Name = "User3" } } );

			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );

			var result = controller.Edit( currentUser ) as ViewResult;
			var model = result.ViewData.Model as AccountController.UserData;

			Assert.IsNotNull( model.Friends );
			Assert.AreEqual( model.Friends.Count, 3 );
			Assert.IsTrue( model.Friends["User1"] );
			Assert.IsFalse( model.Friends["User2"] );
			Assert.IsTrue( model.Friends["User3"] );
		}

		[TestMethod]
		public void EditModelFriends_ShouldNotContainCurrentUser()
		{
			User user = new User { Name = "friendstest" };
			_service.Setup( x => x.GetUser( It.IsAny<string>() ) ).Returns( user );
			_service.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Name = "User1" }, new User { Name = "User2" }, new User { Name = "User3" }, user } );
			_service.Setup( x => x.GetFriends( user ) ).Returns( new List<User> { new User { Name = "User1" }, new User { Name = "User3" } } );
			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );

			var model = ((ViewResult)controller.Edit( currentUser )).ViewData.Model as AccountController.UserData;

			Assert.IsFalse( model.Friends.ContainsKey( user.Name ) );
		}

		[TestMethod]
		public void AddFriend_ShouldCallAddFriendInServiceWithCurrentUserAndReturnJSON()
		{
			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( "someuser", "Forms" ), null );

			var result = controller.AddFriend( currentUser, "newFriend" ) as JsonResult;

			Assert.IsNotNull( result );
			_service.Verify( x => x.AddFriend( "someuser", "newFriend" ), Times.Once() );
		}

		[TestMethod]
		public void RemoveFriend_ShouldCallAddFriendInServiceWithCurrentUserAndReturnJSON()
		{
			IPrincipal currentUser = new GenericPrincipal( new GenericIdentity( "someuser", "Forms" ), null );

			var result = controller.RemoveFriend( currentUser, "notAFriendAnymore" ) as JsonResult;

			Assert.IsNotNull( result );
			_service.Verify( x => x.RemoveFriend( "someuser", "notAFriendAnymore" ), Times.Once() );
		}


		#region Login
		[TestMethod]
		public void AccountController_Index_Redirects_To_Login()
		{
			ActionResult result = controller.Index();
			Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );

			RedirectToRouteResult renderResult = result as RedirectToRouteResult;
			Assert.AreEqual( "Login", renderResult.RouteValues["action"] );
		}

		[TestMethod]
		public void AccountController_Has_a_Login_Method_That_Renders_The_LoginView()
		{
			ActionResult result = controller.Login();
			Assert.IsInstanceOfType( result, typeof( ViewResult ) );

			ViewResult renderResult = result as ViewResult;
			Assert.AreEqual( "Login", renderResult.ViewName );
		}

		[TestMethod]
		public void AccountController_Authenticate_Action_Redirects_For_User1_To_TestUrl()
		{
			_service.Setup( x => x.ValidateUser( It.IsAny<string>(), It.IsAny<string>() ) ).Returns( true );
			AccountController controller = new TestableAccountController( _service.Object );

			ActionResult result = controller.Authenticate( "User 1", "pwd1", "testurl" );

			Assert.IsInstanceOfType( result, typeof( RedirectResult ) );

			RedirectResult redirectResult = result as RedirectResult;
			Assert.AreEqual( "testurl", redirectResult.Url );
		}

		[TestMethod]
		public void AccountController_Authenticate_Redirects_To_Login_For_Bad_Login()
		{
			ActionResult result = controller.Authenticate( "User 1", "The wrong password", "testurl" );

			Assert.IsInstanceOfType( result, typeof( RedirectToRouteResult ) );
			Assert.IsNotNull( controller.TempData["loginmessage"] );

		}

		#endregion
	}

	internal class TestableAccountController : AccountController
	{
		internal TestableAccountController( IUserService service )
			: base( service )
		{
		}

		internal TestableAccountController() : base() { }

		protected override void PerformFormsAuthentication( string userName )
		{
			//Do nothing - we can't test FormsAuthentication anyway.
		}
	}
}
