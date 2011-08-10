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
using Moq;

namespace WishList.Tests
{
	/// <summary>
	/// Summary description for UserTests
	/// </summary>
	[TestClass]
	public class UserServiceTests
	{
		private Mock<IWishListRepository> rep;
		private UserService service;

		[TestInitialize]
		public void Setup()
		{
			rep = new Mock<IWishListRepository>();
			service = new UserService( rep.Object );
			service.ClearCache();
		}

		[TestMethod]
		public void CreateUser_WillCallRepositoryAndReturnCreatedUser()
		{
			User user = new User { Name = "TestUser1" };
			rep.Setup( x => x.CreateUser( user ) ).Returns( ( User u ) => { u.Id = 5; return u; } );



			User createdUser = service.CreateUser( user );

			Assert.IsNotNull( createdUser, "Created user was null" );
			Assert.AreEqual( createdUser.Id, 5 );

			rep.VerifyAll();
		}

		[TestMethod]
		public void Will_Not_Create_Duplicate_User()
		{
			var user = new User { Name = "TestUser2", Email = "testuser2@example.com", Password = "abc123" };
			var alreadyCalled = false;
			rep.Setup( x => x.CreateUser( user) ).Returns( ( User u ) =>
			{
				if (alreadyCalled)
					throw new InvalidOperationException();

				u.Id = 5;
				alreadyCalled = true;
				return u;
			} );

			var createdUser1 = service.CreateUser( user );
			var createdUser2 = service.CreateUser( user );

			Assert.IsNotNull( createdUser1, "User 1 was not created" );
			Assert.IsNull( createdUser2, "Duplicate user creation did not return null" );
		}

		[TestMethod]
		public void WhenCacheIsClearGetUsers_WillGetUsersFromRepository()
		{

			rep.Setup( x => x.GetUsers() ).Returns( new List<User> { new User(), new User() }.AsQueryable() );

			var users = service.GetUsers();

			Assert.IsTrue( users.Count > 0 );
			rep.VerifyAll();
		}

		[TestMethod]
		public void Can_Get_User_1_From_Service_By_Id()
		{
			rep.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Id = 1 } }.AsQueryable() );

			var user1 = service.GetUser( 1 );
			Assert.IsNotNull( user1, "User was null" );
			Assert.AreEqual<int>( 1, user1.Id, "User Id was not 1" );
		}

		[TestMethod]
		public void Can_Get_User_1_From_Service_By_Name()
		{
			rep.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Id = 1, Name = "User 1" } }.AsQueryable() );

			User user1 = service.GetUser( "User 1" );
			Assert.IsNotNull( user1, "User was null" );
			Assert.AreEqual<int>( 1, user1.Id, "User Id was not 1" );
		}

		[TestMethod]
		public void GetUserByName_IsNotCaseSensitive()
		{
			rep.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Id = 1, Name = "User1" } }.AsQueryable() );

			var user = service.GetUser( "uSEr1" );

			Assert.IsNotNull( user );
		}

		[TestMethod]
		public void GetUserByNameReturnsNull_WhenUserDoesNotExist()
		{
			rep.Setup( x => x.GetUsers() ).Returns( new List<User>().AsQueryable() );

			var user = service.GetUser( "foo" );

			Assert.IsNull( user );
		}

		[TestMethod]
		public void WhenRepositoryValidateUserIsTrue_ValidateUserIsTrue()
		{
			rep.Setup( x => x.ValidateUser( "User 1", "pwd1" ) ).Returns( true );

			var status = service.ValidateUser( "User 1", "pwd1" );

			Assert.IsTrue( status, "User could not log on" );
			rep.VerifyAll();
		}

		[TestMethod]
		public void WhenRepositoryValidateUserIsFalse_ValidateUserIsFalse()
		{
			rep.Setup( x => x.ValidateUser( "User 1", "wrong password" ) ).Returns( false );

			var status = service.ValidateUser( "User 1", "wrong password" );

			Assert.IsFalse( status, "User could log on with wrong password" );
			rep.VerifyAll();
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "Empty password did not cause exception" )]
		public void Service_Will_Not_Accept_Blank_Password()
		{
			service.ValidateUser( "User 1", string.Empty );
		}

		[TestMethod]
		[ExpectedException( typeof( InvalidOperationException ), "Empty username did not cause exception" )]
		public void Service_Will_Not_Accept_Blank_Username()
		{
			service.ValidateUser( string.Empty, "password" );
		}

		[TestMethod]
		public void GetApprovalTicket_WillCallRepository()
		{
			Guid ticket = Guid.NewGuid();
			rep.Setup( x => x.GetApprovalTicket( It.IsAny<string>() ) ).Returns( ticket );

			var result = service.GetApprovalTicket( "someuser" );

			Assert.AreEqual( ticket, result );
			rep.VerifyAll();
		}

		[TestMethod]
		public void WhenUserExists_UpdateUserWillCallRepositoryAndReturnUser()
		{
			rep.Setup( x => x.GetUsers() ).Returns( new List<User> { new User { Id = 17, Email = "oldemail@example.com", Name = "testuser" } }.AsQueryable() );
			var user = new User
						{
							Id = 17,
							Email = "newemail@example.com",
							Name = "testuser"
						};
			rep.Setup( x => x.UpdateUser( It.IsAny<User>() ) ).Returns( (User x) => x );

			var updatedUser = service.UpdateUser( user );

			Assert.IsNotNull( updatedUser );
			Assert.AreEqual( user, updatedUser );
			rep.Verify( x => x.UpdateUser( user ), Times.Once() );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ), "Blank email did not cause exception" )]
		public void Cannot_Update_User_With_Blank_Email()
		{
			var user = new User { Email = string.Empty };
			service.UpdateUser( user );
		}

		[TestMethod]
		public void UpdatePassword_WillCallRepository()
		{
			var username = "username";
			var password = "password";
			service.UpdatePassword( username, password );

			rep.Verify( x => x.SetPassword( username, password ), Times.Once() );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ), "Blank password did not cause exception" )]
		public void Cannot_Set_Blank_Password()
		{
			service.UpdatePassword( "username", string.Empty );
		}


		[TestMethod]
		public void GetFriends_Will_Get_Friends_From_Repository()
		{
			var user = new User();
			rep.Setup( x => x.GetFriends( user ) ).Returns( new List<User> { new User(), new User() }.AsQueryable() );

			var result = service.GetFriends( user );

			Assert.IsNotNull( result );
			Assert.AreEqual( 2, result.Count );
			rep.VerifyAll();
		}

		[TestMethod]
		public void WhenUserAndFriendExist_AddFriendWillCallAddFriendInRepository()
		{
			rep.Setup( r => r.GetUsers() ).Returns( new List<User> { new User { Name = "user" }, new User { Name = "friend" } }.AsQueryable() );

			service.AddFriend( "user", "friend" );

			rep.Verify( r => r.AddFriend( It.Is<User>( x => x.Name == "user" ), It.Is<User>( x => x.Name == "friend" ) ) );
		}

		[TestMethod]
		public void WhenUserAndFriendExist_RemoveFriendWillCallRemoveFriendInRepository()
		{
			rep.Setup( r => r.GetUsers() ).Returns( new List<User> { new User { Name = "user" }, new User { Name = "friend" } }.AsQueryable() );

			service.RemoveFriend( "user", "friend" );

			rep.Verify( r => r.RemoveFriend( It.Is<User>( x => x.Name == "user" ), It.Is<User>( x => x.Name == "friend" ) ), Times.Once() );
		}
	}
}
