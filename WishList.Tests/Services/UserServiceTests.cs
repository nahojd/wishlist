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
using NSubstitute;

namespace WishList.Tests
{
	/// <summary>
	/// Summary description for UserTests
	/// </summary>
	[TestClass]
	public class UserServiceTests
	{
		private IWishListRepository rep;
		private UserService service;

		[TestInitialize]
		public void Setup()
		{
			rep = Substitute.For<IWishListRepository>();
			service = new UserService( rep );
			service.ClearCache();
		}

		[TestMethod]
		public void CreateUser_WillCallRepositoryAndReturnCreatedUser()
		{
			User user = new User { Name = "TestUser1" };
			rep.CreateUser( user ).Returns( x =>
			{
				var u = x.Arg<User>();
				u.Id = 5;
				return u;
			} );

			User createdUser = service.CreateUser( user );

			Assert.IsNotNull( createdUser, "Created user was null" );
			Assert.AreEqual( createdUser.Id, 5 );

			rep.Received().CreateUser( user );
		}

		[TestMethod]
		public void Will_Not_Create_Duplicate_User()
		{
			User user = new User { Name = "TestUser2", Email = "testuser2@example.com", Password = "abc123" };
			var alreadyCalled = false;
			rep.CreateUser( user ).Returns( x =>
			{
				if (alreadyCalled)
					throw new InvalidOperationException();

				var u = x.Arg<User>();
				u.Id = 5;
				alreadyCalled = true;
				return u;
			} );

			User createdUser1 = service.CreateUser( user );
			User createdUser2 = service.CreateUser( user );

			Assert.IsNotNull( createdUser1, "User 1 was not created" );
			Assert.IsNull( createdUser2, "Duplicate user creation did not return null" );
		}

		[TestMethod]
		public void WhenCacheIsClearGetUsers_WillGetUsersFromRepository()
		{

			rep.GetUsers().Returns( new List<User> { new User(), new User() }.AsQueryable() );

			var users = service.GetUsers();

			Assert.IsTrue( users.Count > 0 );
			rep.Received().GetUsers();
		}

		[TestMethod]
		public void Can_Get_User_1_From_Service_By_Id()
		{
			rep.GetUsers().Returns( new List<User> { new User { Id = 1 } }.AsQueryable() );

			User user1 = service.GetUser( 1 );
			Assert.IsNotNull( user1, "User was null" );
			Assert.AreEqual<int>( 1, user1.Id, "User Id was not 1" );
		}

		[TestMethod]
		public void Can_Get_User_1_From_Service_By_Name()
		{
			rep.GetUsers().Returns( new List<User> { new User { Id = 1, Name = "User 1" } }.AsQueryable() );

			User user1 = service.GetUser( "User 1" );
			Assert.IsNotNull( user1, "User was null" );
			Assert.AreEqual<int>( 1, user1.Id, "User Id was not 1" );
		}

		[TestMethod]
		public void WhenRepositoryValidateUserIsTrue_ValidateUserIsTrue()
		{
			rep.ValidateUser( "User 1", "pwd1" ).Returns( true );

			var status = service.ValidateUser( "User 1", "pwd1" );

			Assert.IsTrue( status, "User could not log on" );
			rep.Received().ValidateUser( "User 1", "pwd1" );
		}

		[TestMethod]
		public void WhenRepositoryValidateUserIsFalse_ValidateUserIsFalse()
		{
			rep.ValidateUser( "User 1", "wrong password" ).Returns( false );

			var status = service.ValidateUser( "User 1", "wrong password" );

			Assert.IsFalse( status, "User could log on with wrong password" );
			rep.Received().ValidateUser( "User 1", "wrong password" );
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
			rep.GetApprovalTicket( Arg.Any<string>() ).Returns( ticket );

			var result = service.GetApprovalTicket( "someuser" );

			Assert.AreEqual( ticket, result );
			rep.Received().GetApprovalTicket( Arg.Any<string>() );
		}

		[TestMethod]
		public void WhenUserExists_UpdateUserWillCallRepositoryAndReturnUser()
		{
			rep.GetUsers().Returns( new List<User> { new User { Id = 17, Email = "oldemail@example.com", Name = "testuser" } }.AsQueryable() );
			User user = new User
			{
				Id = 17,
				Email = "newemail@example.com",
				Name = "testuser"
			};
			rep.UpdateUser( Arg.Any<User>() ).Returns( x => x.Arg<User>() );

			var updatedUser = service.UpdateUser( user );

			Assert.IsNotNull( updatedUser );
			Assert.AreEqual( user, updatedUser );
			rep.Received().UpdateUser( user );
		}

		[TestMethod]
		[ExpectedException( typeof( ArgumentException ), "Blank email did not cause exception" )]
		public void Cannot_Update_User_With_Blank_Email()
		{
			User user = new User { Email = string.Empty };
			service.UpdateUser( user );
		}

		[TestMethod]
		public void UpdatePassword_WillCallRepository()
		{
			string username = "username";
            string password = "password";
            service.UpdatePassword( username, password );

			rep.Received().SetPassword( username, password );
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
			rep.GetFriends( user ).Returns( new List<User> { new User(), new User() }.AsQueryable() );

			var result = service.GetFriends( user );

			Assert.IsNotNull( result );
			Assert.AreEqual( 2, result.Count );
			rep.Received().GetFriends( user );
		}

		[TestMethod]
		public void WhenUserAndFriendExist_AddFriendWillCallAddFriendInRepository()
		{
			rep.GetUsers().Returns( new List<User> { new User { Name = "user" }, new User { Name = "friend" } }.AsQueryable() );

			service.AddFriend( "user", "friend" );

			rep.Received().AddFriend( Arg.Is<User>( x => x.Name == "user" ), Arg.Is<User>( x => x.Name == "friend" ) );
		}

		[TestMethod]
		public void WhenUserAndFriendExist_RemoveFriendWillCallRemoveFriendInRepository()
		{
			rep.GetUsers().Returns( new List<User> { new User { Name = "user" }, new User { Name = "friend" } }.AsQueryable() );

			service.RemoveFriend( "user", "friend" );

			rep.Received().RemoveFriend( Arg.Is<User>( x => x.Name == "user" ), Arg.Is<User>( x => x.Name == "friend" ) );
		}
	}
}
