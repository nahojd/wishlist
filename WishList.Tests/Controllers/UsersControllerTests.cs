using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WishList.WebUI.Controllers;
using WishList.Services;
using Moq;
using WishList.Data;
using System.Web.Mvc;
using System.Security.Principal;
using WishList.WebUI.ViewModels;
using System.Web.Routing;

namespace WishList.Tests.Controllers
{
	[TestClass]
	public class UsersControllerTests
	{
		private GenericPrincipal currentUser;
		private User user;
		UsersController controller;
		Mock<IUserService> service;

		[TestInitialize]
		public void Setup()
		{
			service = new Mock<IUserService>();

			user = new User { Name = "username" };
			service.Setup( x => x.GetUser( It.IsAny<string>() ) ).Returns( user );
			service.Setup( x => x.GetFriends( user ) ).Returns( new List<User> { new User { Id = 1 }, new User { Id = 2, Name = "User2" }, new User { Id = 3 } } );
			currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );

			controller = new UsersController( service.Object );
			SetupRouteData( controller );
		}

		private void SetupRouteData( UsersController controller )
		{
			var routeData = new RouteData();
			routeData.Values.Add( "id", "User2" );
			controller.ControllerContext = new ControllerContext();
			controller.ControllerContext.RouteData = routeData;
		}

		[TestMethod]
		public void ListFriends_ShouldGetFriendsFromServiceAndReturnPlusCurrentUserInViewModel()
		{


			

			var result = controller.ListFriends( currentUser ) as ViewResult;

			var model = result.ViewData.Model as ListFriendsModel;
			Assert.IsNotNull( model );
			Assert.AreEqual( 4, model.Friends.Count );
			Assert.IsTrue( model.Friends.Any( u => u.Name == user.Name ) );
		}

		[TestMethod]
		public void WhenActionIsShowUser_SelectedUserShouldBeSetToUserShown()
		{


			//User user = new User { Name = "username" };
			//service.GetUser( Arg.Any<string>() ).Returns( user );
			//service.GetFriends( Arg.Is<User>( user ) ).Returns(  );
			//var currentUser = new GenericPrincipal( new GenericIdentity( user.Name, "Forms" ), null );

			var result = controller.ListFriends( currentUser ) as ViewResult;

			var model = result.ViewData.Model as ListFriendsModel;
			Assert.AreEqual( "User2", model.SelectedUsername );
		}
	}
}
