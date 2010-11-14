using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.Data;
using WishList.Data.DataAccess;
using WishList.Data.Filters;
using WishList.Services;
using System.Web.Security;

namespace WishList.Tests
{
	/// <summary>
	/// Summary description for UserTests
	/// </summary>
	[TestClass]
	public class UserTests
	{
		private UserService Service
		{
			get;
			set;
		}

		[TestInitialize]
		public void Setup()
		{
			IWishListRepository rep = new TestWishListRepository();
			Service = new UserService( rep );
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void WishListService_Can_Create_User()
		{
			string password = "p4ssw0rd";
			User user = new User
			{
				Name = "TestUser1",
				Email = "testuser1@example.com",
				Password = password,
				NotifyOnChange = true
			};

			User createdUser = Service.CreateUser( user );

			Assert.IsNotNull( createdUser, "Created user was null" );
			Assert.IsTrue( createdUser.Id > 0, "User id was not greater than 0" );
			Assert.AreEqual( user.Name, createdUser.Name, "Created user name did not match" );
			Assert.AreEqual( user.Email, createdUser.Email, "Created user email did not match" );
			Assert.AreEqual( user.NotifyOnChange, createdUser.NotifyOnChange, "NotifyOnChange did not match" );
			Assert.IsTrue( Service.ValidateUser( user.Name, password ), "User password was not valid" );

			User loadedUser = (from u in Service.GetUsers()
							   where u.Name == user.Name
							   select u).SingleOrDefault();

			Assert.IsNotNull( loadedUser, "Created user could not be loaded" );
		}

		[TestMethod]
		public void WishListService_Will_Not_Create_Duplicate_User()
		{
			User user = new User { Name = "TestUser2", Email = "testuser2@example.com", Password = "abc123" };

			User createdUser1 = Service.CreateUser( user );
			User createdUser2 = Service.CreateUser( user );

			Assert.IsNotNull( createdUser1, "User 1 was not created" );
			Assert.IsNull( createdUser2, "Duplicate user creation did not return null" );
		}

		[TestMethod]
		public void WishListService_Can_Get_Users_From_Service()
		{
			IList<User> users = Service.GetUsers();
			Assert.IsTrue( users.Count > 0 );
		}

		[TestMethod]
		public void WishListService_Can_Get_User_1_From_Service_By_Id()
		{
			User user1 = Service.GetUser( 1 );
			Assert.IsNotNull( user1, "User was null" );
			Assert.AreEqual<int>( 1, user1.Id, "User Id was not 1" );
		}

		[TestMethod]
		public void WishListService_Can_Get_User_1_From_Service_By_Name()
		{
			User user1 = Service.GetUser( "User 1" );
			Assert.IsNotNull( user1, "User was null" );
			Assert.AreEqual<int>( 1, user1.Id, "User Id was not 1" );
		}

		[TestMethod]
		public void Service_Can_Log_on_User_1()
		{
			bool status = Service.ValidateUser( "User 1", "pwd1" );
			Assert.IsTrue( status, "User could not log on" );

			status = Service.ValidateUser( "User 1", "wrong password" );
			Assert.IsFalse( status, "User could log on with wrong password" );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "Empty password did not cause exception" )]
		public void Service_Will_Not_Accept_Blank_Password()
		{
			Service.ValidateUser( "User 1", string.Empty );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "Empty username did not cause exception" )]
		public void Service_Will_Not_Accept_Blank_Username()
		{
			Service.ValidateUser( string.Empty, "password" );
		}

		[TestMethod]
		public void Can_Get_Approval_Ticket()
		{
			User user = Service.CreateUser( new User { Name = "ApproveTicketTestUser", Email = "approvetickettest@test.com", Password = "approveme" } );
			Guid? ticket = Service.GetApprovalTicket( user.Name );

			Assert.IsTrue( ticket.HasValue, "Ticket was null" );
		}

		[TestMethod]
		public void Can_Update_User()
		{
			int updateUserId = 17;
			User user = Service.GetUser( updateUserId );

			string newEmail = "newemail@example.com";
			user.Email = newEmail;
			user.NotifyOnChange = true;
			user = Service.UpdateUser( user );


			Assert.AreEqual<string>( newEmail, user.Email, "Email was not updated!" );
			Assert.AreEqual( true, user.NotifyOnChange, "NotifyOnChange was not updated!" );
			Assert.AreEqual<int>( updateUserId, user.Id, "Id was changed while updating!" );

			User loadedUser = Service.GetUser( 17 );
			Assert.AreEqual<string>( newEmail, loadedUser.Email, "Email was not updated when loading user" );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ), "Blank email did not cause exception" )]
		public void Cannot_Update_User_With_Blank_Email()
		{
			int updateUserId = 17;
			User user = Service.GetUser( updateUserId );

			user.Email = string.Empty;
			Service.UpdateUser( user );
		}

		[TestMethod]
		public void Can_Update_Password()
		{
			User user = Service.GetUser( 5 );
			string newPassword = "newPassword";

			Service.UpdatePassword( user.Name, newPassword );

			user = Service.GetUser( 5 );
			Assert.IsTrue( Service.ValidateUser( user.Name, newPassword ), "New password did not work" );
			Assert.IsTrue( Service.ValidateUser( user.Name, newPassword ), "Could not log on with new password" );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ), "Blank password did not cause exception" )]
		public void Cannot_Set_Blank_Password()
		{
			User user = Service.GetUser( 5 );
			string newPassword = string.Empty;

			Service.UpdatePassword( user.Name, newPassword );
		}


	}
}
