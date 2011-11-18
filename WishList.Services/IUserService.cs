using System;
using WishList.Data;
using System.Collections.Generic;
namespace WishList.Services
{
	public interface IUserService
	{
		void ApproveUser( string username, Guid ticket );
		User CreateUser( User user );
		void DeleteUser( string username );
		Guid? GetApprovalTicket( string username );


		User GetUser( int userId );
		User GetUser( string username );
		IList<User> GetUsers();
		
        void UpdatePassword( string username, string newPassword );
		User UpdateUser( User user );
		bool ValidateUser( string userName, string password );

		//FriendService
		IList<User> GetFriends( User user );
		void AddFriend( string username, string friendname );
		void RemoveFriend( string username, string friendname );
	}
}
