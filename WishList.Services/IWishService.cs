using System;
namespace WishList.Services
{
	public interface IWishService
	{
		System.Collections.Generic.IList<WishList.Data.Wish> GetLatestActivityList( int userId );
		System.Collections.Generic.IList<WishList.Data.Wish> GetShoppingList( int userId );
		WishList.Data.Wish GetWish( int id );
		WishList.Data.WishList GetWishList( int ownerId );
		WishList.Data.WishList GetWishList( string userName );
		void RemoveWish( WishList.Data.Wish wish );
		void SaveWish( WishList.Data.Wish wish, bool suppressNotifications );
	}
}
