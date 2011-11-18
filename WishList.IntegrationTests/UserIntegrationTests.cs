using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.Data.DataAccess;
using WishList.Data;
using WishList.Services;
using System.Web.Security;
using System.Configuration;
using DB = WishList.SqlRepository.Data;

namespace WishList.IntegrationTests
{
	/// <summary>
	/// Summary description for UserIntegrationTests
	/// </summary>
	[TestClass]
	public class UserIntegrationTests
	{
		private SqlRepository.LinqWishListDataContext dataContext;
		UserService service;
		SqlWishListRepository rep;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void SetUp()
		{
#if DEBUG
			dataContext = new SqlRepository.LinqWishListDataContext( ConfigurationManager.ConnectionStrings["LocalTestDb"].ConnectionString );
#else 
			dataContext = new SqlRepository.LinqWishListDataContext();
#endif
			dataContext.Connection.Open();
			var transaction = dataContext.Connection.BeginTransaction();
			dataContext.Transaction = transaction;
			rep = new SqlWishListRepository( dataContext );
			service = new UserService( rep );

			PopulateDB();
		}

		private void PopulateDB()
		{
			var firstUser = new DB.User { Name = "First", Email = "first@example.com" };
			var secondUser = new DB.User { Name = "Second", Email = "second@example.com" };
			dataContext.Users.InsertOnSubmit( firstUser );
			dataContext.Users.InsertOnSubmit( secondUser );

			dataContext.SubmitChanges();
		}

		[TestCleanup]
		public void TearDown()
		{
			dataContext.Transaction.Rollback();
			dataContext.Connection.Close();
			rep.Dispose();
		}

		/// <summary>
		/// Creates a new user
		/// </summary>
		/// <param name="username"></param>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		private User CreateUser( string username, string email, string password )
		{
			return rep.CreateUser( new User { Name = username, Email = email, Password = password } );
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
			var users = rep.GetUsers().ToList<User>();

			Assert.IsNotNull( users, "Users was null" );
			Assert.IsTrue( users.Count > 0, "User count was 0" );
		}

		[TestMethod]
		public void SqlRepository_Can_Create_User()
		{
			var user = new User
						{
							Name = "CreateTestUser",
							Email = "createtest@example.com",
							Password = "CreateTestPassword",
							NotifyOnChange = true
						};

			var createdUser = rep.CreateUser( user );

			Assert.IsNotNull( createdUser, "Created user was null" );
			Assert.IsTrue( createdUser.Id > 0, "User id was not greater than 0" );
			Assert.AreEqual( user.Name, createdUser.Name, "Created user's name did not match" );

			var loadedUser = dataContext.Users.Where( u => u.UserId == createdUser.Id ).Single();
			Assert.IsNotNull( loadedUser, "Could not load created user from repository" );
			Assert.AreEqual( user.Name, loadedUser.Name, "Created user's name was wrong" );
			Assert.AreEqual( user.Email, loadedUser.Email, "Created user's email was wrong" );
			Assert.AreEqual( user.NotifyOnChange, loadedUser.NotifyOnChange, "Created user's NotifyOnChange was wrong" );
		}

		[TestMethod]
		public void CreateUser_WillSetSaltAndPasswordHash()
		{
			var user = new User { Name = "Foo", Email = "foo@example.com", Password = "foobar" };
			rep.CreateUser( user );

			var createdUser = dataContext.Users.Where( u => u.Name == user.Name ).Single();
			var expectedHash = SqlWishListRepository.GetHash(user.Password, createdUser.Salt);
			Assert.AreEqual( expectedHash, createdUser.PasswordHash );
		}

		[TestMethod]
		public void SqlRepository_Can_Update_User()
		{
			var user = CreateUser( "UpdateTestUser", "updatetest@test.com", "updatetest" );
			int updateUserId = user.Id;

			var newEmail = "updatedemail@test.com";

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
		public void WhenUsernameExistInDb_UsernameIsUniqueWillReturnFalse()
		{
			var result = rep.UsernameIsUnique( "First" );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void WhenUsernameDoesNotExistInDb_UsernameIsUniqueWilLReturnTrue()
		{
			var result = rep.UsernameIsUnique( "askjdhalksjdhlasjkdhl" );
			Assert.IsTrue( result );
		}

		[TestMethod]
		//[ExpectedException( typeof( InvalidOperationException ), "InvalidOperationException was not thrown" )]
		public void SqlRepository_Will_Not_Create_Duplicate_Users()
		{
			var user = new User { Name = "DuplicateUser", Password = "password", Email = "duplicateuser@example.com" };

			var user1 = rep.CreateUser( user );
			Assert.IsNotNull( user1 );
			try
			{
				rep.CreateUser( user );
			}
			catch (InvalidOperationException ex)
			{
				Assert.AreEqual( "A user with that name already exists!", ex.Message );
				return;
			}
			Assert.Fail();
		}

		[TestMethod]
		public void ValidateUser_UserIsNotValidated_WillReturnFalse()
		{
			var user = CreateUser( "LogOnTestUser", "logontest@example.com", "l0g0nt3st" );

			var result = rep.ValidateUser( user.Name, user.Password );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void ValidateUser_PasswordIsWrong_WillReturnFalse()
		{
			var user = CreateUser( "LogOnTestUser", "logontest@example.com", "l0g0nt3st" );
			rep.ApproveUser( user.Name, rep.GetApprovalTicket( user.Name ).Value );

			var result = rep.ValidateUser( user.Name, "wrongpassword" );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void ValidateUser_UsernameDoesNotExist_WillReturnFalse()
		{
			var result = rep.ValidateUser( "Badusername", "whateverpassword" );

			Assert.IsFalse( result );
		}

		[TestMethod]
		public void ValidateUser_UserIsApprovedAndUsernameAndPasswordAreCorrect_WillReturnTrue()
		{
			var user = CreateUser( "LogOnTestUser", "logontest@example.com", "l0g0nt3st" );
			rep.ApproveUser( user.Name, rep.GetApprovalTicket( user.Name ).Value );

			var result = rep.ValidateUser( user.Name, user.Password );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void ValidateUser_IsNotCaseSensitiveOnUsername()
		{
			var user = CreateUser( "LogOnTestUser", "logontest@example.com", "l0g0nt3st" );
			rep.ApproveUser( user.Name, rep.GetApprovalTicket( user.Name ).Value );

			var result = rep.ValidateUser( user.Name.ToUpper(), user.Password );

			Assert.IsTrue( result );
		}

		[TestMethod]
		public void ValidateUser_PasswordIsEmpty_ReturnsFalse()
		{
			var result = rep.ValidateUser( "test", null );

			Assert.IsFalse( result );
		}

		//[TestMethod]
		//public void SqlRepository_Can_Log_On_User()
		//{
		//    var password = "l0g0nt3st";
		//    var user = CreateUser( "LogOnTestUser", "logontest@example.com", password );

		//    bool logonStatus = rep.ValidateUser( user.Name, password );
		//    Assert.IsFalse( logonStatus, "User could log on before being activated" );

		//    var membershipUser = membership.GetUser( user.Name );
		//    membershipUser.IsApproved = true;
		//    membership.UpdateUser( membershipUser );

		//    logonStatus = rep.ValidateUser( user.Name, password );
		//    Assert.IsTrue( logonStatus, "User could not log on" );
		//}

		[TestMethod]
		public void Service_Can_Approve_User()
		{
			var user = CreateUser( "ApproveTestUser", "approvetest@test.com", "approveme" );
			Assert.IsNotNull( user, "Created user was null" );

			//var membershipUser = membership.GetUser( user.Name );
			//Assert.IsNotNull( membershipUser, "Membership user was null" );
			//Assert.IsFalse( membershipUser.IsApproved, "Created membership user was already approved" );

			var ticket = rep.GetApprovalTicket( user.Name );
			service.ApproveUser( user.Name, ticket.Value );
			//membershipUser = membership.GetUser( user.Name );
			//Assert.IsTrue( membershipUser.IsApproved, "Membership user was not approved" );

			ticket = rep.GetApprovalTicket( user.Name );
			Assert.IsFalse( ticket.HasValue, "Approved user had a ticket" );

			//DeleteUser( user.Name );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "ApproveUser did not throw exception even though the ticket was wrong" )]
		public void Service_Will_Not_Approve_User_With_Wrong_Ticket()
		{
			var user = CreateUser( "ApproveTestFailUser", "ApproveTestFailUser@test.com", "approveme" );

			//MembershipUser membershipUser = membership.GetUser( user.Name );

			service.ApproveUser( user.Name, Guid.NewGuid() );
		}

		[TestMethod]
		public void SqlRepository_Can_Get_Approval_Ticket_For_User()
		{
			var user = CreateUser( "ApprovalTicketTestUser", "ApprovalTicketTestUser@test.com", "approveme" );
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
		[ExpectedException( typeof( InvalidOperationException ) )]
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

		[TestMethod]
		public void WhenNoFriendsExist_GetFriendsWillReturnEmptyList()
		{
			var user = CreateUser( "friendtestuser", "test@example.com", "foobar" );

			var result = rep.GetFriends( user );

			Assert.IsNotNull( result );
			Assert.AreEqual( 0, result.Count() );
		}
	}
}
