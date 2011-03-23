using System;
using System.Collections.Generic;
using System.Linq;
using WishList.Data;
using WishList.Data.Filters;
using WishList.Data.DataAccess;
using System.Web;

namespace WishList.Services
{
	public class UserService : IUserService
	{
		IWishListRepository _repository = null;
		private static readonly string _userListCacheKey = "UserList";

		public UserService()
			: this( new SqlWishListRepository() ) { }

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
			List<User> userList = new List<User>( UserList.Count );
			foreach (User u in UserList)
			{
				userList.Add( u.Clone() );
			}
			return userList;
		}

		public IList<User> GetFriends( User user )
		{
			return _repository.GetFriends( user ).ToList();
		}

		public void AddFriend( string username, string friendname )
		{
			var user = GetUser( username );
			var friend = GetUser( friendname );

			_repository.AddFriend( user, friend );
		}

		public void RemoveFriend( string username, string friendname )
		{
			var user = GetUser( username );
			var friend = GetUser( friendname );

			_repository.RemoveFriend( user, friend );
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
			if (UserList.Any( u => u.Id == userId ))
				return UserList.WithId( userId ).Clone();
			return null;
		}

		public User GetUser( string username )
		{
			if ((UserList.Any( u => u.Name.Equals( username, StringComparison.InvariantCultureIgnoreCase ) )))
				return UserList.WithName( username ).Clone();
			return null;
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

		public void ClearCache()
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
