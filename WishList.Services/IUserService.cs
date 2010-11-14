using System;
namespace WishList.Services
{
	public interface IUserService
	{
		void ApproveUser( string username, Guid ticket );
		WishList.Data.User CreateUser( WishList.Data.User user );
		void DeleteUser( string username );
		Guid? GetApprovalTicket( string username );
		WishList.Data.User GetUser( int userId );
		WishList.Data.User GetUser( string username );
		System.Collections.Generic.IList<WishList.Data.User> GetUsers();
		void UpdatePassword( string username, string newPassword );
		WishList.Data.User UpdateUser( WishList.Data.User user );
		bool ValidateUser( string userName, string password );
	}
}
