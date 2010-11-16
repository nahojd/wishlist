using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.Data.DataAccess;
using WishList.Data;
using WishList.Services;
using WishList.Data.Filters;
using System.Web.Security;

namespace WishList.IntegrationTests
{
	/// <summary>
	/// Summary description for UserIntegrationTests
	/// </summary>
	[TestClass]
	public class UserIntegrationTests
	{
		UserService service;
		SqlWishListRepository rep;
		private List<string> usersToDelete;

		public UserIntegrationTests()
		{
			rep = new SqlWishListRepository();
			service = new UserService( rep );
			usersToDelete = new List<string>();
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void SetUp()
		{

		}

		[TestCleanup]
		public void TearDown()
		{
			//Clear up users created in this test
			usersToDelete.ForEach( DeleteUser );
		}

		/// <summary>
		/// Creates a new user and adds it to the list of users to be deleted on teardown
		/// </summary>
		/// <param name="username"></param>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		private User CreateUser( string username, string email, string password )
		{
			User user = rep.CreateUser( new User { Name = username, Email = email, Password = password } );
			usersToDelete.Add( username );
			return user;
		}

		private static void DeleteUser( string userName )
		{
			using (SqlRepository.LinqWishListDataContext db = new WishList.SqlRepository.LinqWishListDataContext())
			{
				var removeUserQuery = from u in db.Users
									  where u.Name == userName
									  select u;
				db.Users.DeleteAllOnSubmit( removeUserQuery );

				db.SubmitChanges();
			}

			Membership.DeleteUser( userName );
		}

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
		public void SqlRepository_Can_Return_Users()
		{
			IList<User> users = rep.GetUsers().ToList<User>();

			Assert.IsNotNull( users, "Users was null" );
			Assert.IsTrue( users.Count > 0, "User count was 0" );
		}

		[TestMethod]
		public void SqlRepository_Can_Create_User()
		{
			User user = new User
			{
				Name = "CreateTestUser",
				Email = "createtest@example.com",
				Password = "CreateTestPassword",
				NotifyOnChange = true
			};

			User createdUser = rep.CreateUser( user );

			//Make sure the user is deleted on teardown
			usersToDelete.Add( user.Name );

			Assert.IsNotNull( createdUser, "Created user was null" );
			Assert.IsTrue( createdUser.Id > 0, "User id was not greater than 0" );
			Assert.AreEqual( user.Name, createdUser.Name, "Created user's name did not match" );

			User loadedUser = service.GetUser( createdUser.Id );
			Assert.IsNotNull( loadedUser, "Could not load created user from repository" );
			Assert.AreEqual( user.Name, loadedUser.Name, "Created user's name was wrong" );
			Assert.AreEqual( user.Email, loadedUser.Email, "Created user's email was wrong" );
			Assert.AreEqual( user.NotifyOnChange, loadedUser.NotifyOnChange, "Created user's NotifyOnChange was wrong" );
		}

		[TestMethod]
		public void SqlRepository_Can_Update_User()
		{
			User user = CreateUser( "UpdateTestUser", "updatetest@test.com", "updatetest" );
			int updateUserId = user.Id;

			string newEmail = "updatedemail@test.com";

			user.Email = newEmail;
			user.NotifyOnChange = true;

			user = rep.UpdateUser( user );

			Assert.AreEqual( newEmail, user.Email, "Email was not updated!" );
			Assert.AreEqual( updateUserId, user.Id, "Id was changed while updating!" );
			Assert.AreEqual( true, user.NotifyOnChange, "NotifyOnChange did not change" );
		}

		[TestMethod]
		public void SqlRepository_Can_Update_Password()
		{
			User user = CreateUser( "UpdatePasswordTestUser", "updatepassword@test.com", "oldpassword" );
			rep.ApproveUser( user.Name, rep.GetApprovalTicket( user.Name ).Value );
			string newPassword = "n3w p4ssw0rd";

			rep.SetPassword( user.Name, newPassword );

			Assert.IsTrue( rep.ValidateUser( user.Name, newPassword ), "Could not log on with new password" );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "InvalidOperationException was not thrown" )]
		public void SqlRepository_Will_Not_Create_Duplicate_Users()
		{
			User user = new User { Name = "DuplicateUser", Password = "password", Email = "duplicateuser@example.com" };

			usersToDelete.Add( "DuplicateUser" ); //Make sure the created user is deleted on teardown

			rep.CreateUser( user );
			rep.CreateUser( user );
		}

		[TestMethod]
		public void SqlRepository_Can_Log_On_User()
		{
			string password = "l0g0nt3st";
			User user = CreateUser( "LogOnTestUser", "logontest@example.com", password );

			bool logonStatus = rep.ValidateUser( user.Name, password );
			Assert.IsFalse( logonStatus, "User could log on before being activated" );

			MembershipUser membershipUser = Membership.GetUser( user.Name );
			membershipUser.IsApproved = true;
			Membership.UpdateUser( membershipUser );

			logonStatus = rep.ValidateUser( user.Name, password );
			Assert.IsTrue( logonStatus, "User could not log on" );

			DeleteUser( user.Name );
		}

		[TestMethod]
		public void Service_Can_Approve_User()
		{
			User user = CreateUser( "ApproveTestUser", "approvetest@test.com", "approveme" );
			Assert.IsNotNull( user, "Created user was null" );

			MembershipUser membershipUser = Membership.GetUser( user.Name );
			Assert.IsNotNull( membershipUser, "Membership user was null" );
			Assert.IsFalse( membershipUser.IsApproved, "Created membership user was already approved" );

			Guid? ticket = rep.GetApprovalTicket( user.Name );
			service.ApproveUser( user.Name, ticket.Value );
			membershipUser = Membership.GetUser( user.Name );
			Assert.IsTrue( membershipUser.IsApproved, "Membership user was not approved" );

			ticket = rep.GetApprovalTicket( user.Name );
			Assert.IsFalse( ticket.HasValue, "Approved user had a ticket" );

			DeleteUser( user.Name );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "ApproveUser did not throw exception even though the ticket was wrong" )]
		public void Service_Will_Not_Approve_User_With_Wrong_Ticket()
		{
			User user = CreateUser( "ApproveTestFailUser", "ApproveTestFailUser@test.com", "approveme" );

			MembershipUser membershipUser = Membership.GetUser( user.Name );

			service.ApproveUser( user.Name, Guid.NewGuid() );
		}

		[TestMethod]
		public void SqlRepository_Can_Get_Approval_Ticket_For_User()
		{
			User user = CreateUser( "ApprovalTicketTestUser", "ApprovalTicketTestUser@test.com", "approveme" );
			Guid? ticket = rep.GetApprovalTicket( user.Name );
			Assert.IsTrue( ticket.HasValue, "Approval ticket was null" );
		}

		[TestMethod]
		public void CanAddFriend()
		{
			var user = CreateUser( "addfrienduser", "a@b.se", "p4ssw0rd" );
			var friend1 = CreateUser( "friend11", "c@d.se", "p4ssw0rd" );

			rep.AddFriend( user, friend1 );

			var friends = rep.GetFriends( user );
			Assert.AreEqual( friend1.Name, friends.Single().Name );
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WhenFriendExist_AddFriendWillThrowException()
		{
			var user = CreateUser( "doubleaddfrienduser", "a@b.se", "p4ssw0rd" );
			var friend = CreateUser( "friend123123", "c@d.se", "p4ssw0rd" );

			rep.AddFriend( user, friend );
			rep.AddFriend( user, friend );
		}

		[TestMethod]
		public void CanRemoveFriend()
		{
			var user = CreateUser( "removefrienduser", "a@b.se", "p4ssw0rd" );
			var friend = CreateUser( "friend29", "c@d.se", "p4ssw0rd" );
			rep.AddFriend( user, friend );

			rep.RemoveFriend( user, friend );

			var friends = rep.GetFriends( user );
			Assert.AreEqual( 0, friends.Count() );
		}

		[TestMethod]
		public void CanGetFriends()
		{
			var user = CreateUser( "friendtestuser", "a@b.se", "p4ssw0rd" );
			var friend1 = CreateUser( "friend1", "b@c.se", "p4ssw0rd" );
			var friend2 = CreateUser( "friend2", "d@e.se", "p4ssw0rd" );
			rep.AddFriend( user, friend1 );
			rep.AddFriend( user, friend2 );

			var result = rep.GetFriends( user );

			Assert.IsNotNull( result );
			Assert.AreEqual( 2, result.Count() );
		}
	}
}
