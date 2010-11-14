using System;
using System.Linq;

namespace WishList.Data.DataAccess
{
    public interface IWishListRepository
    {
        IQueryable<User> GetUsers();
        IQueryable<Wish> GetWishes();

		Wish SaveWish( Wish wish );

		void RemoveWish( Wish wish );

		User CreateUser( User user );

		bool ValidateUser( string username, string password );

		void ApproveUser( string username, Guid ticket );

		Guid? GetApprovalTicket( string username );

		User UpdateUser( User user );

		void SetPassword( string username, string password );
	}
}
