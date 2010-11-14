using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishList.Data;
using WishList.Data.Filters;
using WishList.Data.DataAccess;
using System.Web.Caching;
using System.Web;

namespace WishList.Services
{
	public class UserService : WishList.Services.IUserService
	{
		IWishListRepository _repository = null;
		private static readonly string _userListCacheKey = "UserList";

		public UserService( IWishListRepository repository )
		{
			if (repository == null)
			{
				throw new ArgumentNullException( "repository", "Repository cannot be null!" );
			}
			_repository = repository;
		}

		public IList<User> GetUsers()
		{
			//return UserList;
			List<User> userList = new List<User>( UserList.Count );
			foreach( User u in UserList ) {
				userList.Add( u.Clone() );
			}
			return userList;
		}

		public User CreateUser( User user )
		{
			try
			{
				User createdUser = _repository.CreateUser( user );
				ClearCache();
				return createdUser;
			}
			catch (InvalidOperationException)
			{
				return null;
			}

		}

		public void DeleteUser( string username )
		{
			throw new NotImplementedException();
		}

		public User GetUser( int userId )
		{
			return UserList.WithId( userId ).Clone();
		}

		public User GetUser( string username )
		{
			return UserList.WithName( username ).Clone();
		}

		/// <summary>
		/// Log's the user in
		/// </summary>
		public bool ValidateUser( string userName, string password )
		{

			if (string.IsNullOrEmpty( userName ))
				throw new InvalidOperationException( "Need a user name" );

			if (string.IsNullOrEmpty( password ))
				throw new InvalidOperationException( "Need a password" );

			return _repository.ValidateUser( userName, password );
		}

		private IList<User> UserList
		{
			get
			{
				IList<User> userList = HttpRuntime.Cache.Get( _userListCacheKey ) as IList<User>;
				if (userList == null)
				{
					userList = _repository.GetUsers().ToList<User>();
					HttpRuntime.Cache.Insert( _userListCacheKey, userList );
				}
				return userList;
			}
		}

		private void ClearCache()
		{
			HttpRuntime.Cache.Remove( _userListCacheKey );
		}

		public void ApproveUser( string username, Guid ticket )
		{
			_repository.ApproveUser( username, ticket );
		}

		public Guid? GetApprovalTicket( string username )
		{
			return _repository.GetApprovalTicket( username );
		}

		public User UpdateUser( User user )
		{
			if (string.IsNullOrEmpty( user.Email ))
				throw new ArgumentException( "Email cannot be empty", "user" );

			if (UserList.WithName( user.Name ) != null && UserList.WithName( user.Name ).Id != user.Id)
				throw new InvalidOperationException( string.Format( "Another user with the name {0} already exists!", user.Name ) );


			User updatedUser = _repository.UpdateUser( user );
			ClearCache();
			return updatedUser;
		}

		public void UpdatePassword( string username, string newPassword )
		{
			if (string.IsNullOrEmpty( newPassword ))
			{
				throw new ArgumentException( "Password cannot be null or empty", "newPassword" );
			}
			if (string.IsNullOrEmpty( username ))
			{
				throw new ArgumentException( "Username cannot be null or empty", "username" );
			}

			_repository.SetPassword( username, newPassword );
			ClearCache();
		}
	}
}
