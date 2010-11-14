using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishList.Data;
using WishList.Data.Filters;
using WishList.Data.DataAccess;
using System.Net.Mail;

namespace WishList.Services
{
	public class WishService : WishList.Services.IWishService
	{
		IWishListRepository _repository = null;
		IMailService _mailService;

		public WishService( IWishListRepository repository, IMailService mailService )
		{
			if (repository == null)
			{
				throw new ArgumentNullException( "repository", "Repository cannot be null!" );
			}
			_repository = repository;
			_mailService = mailService;
		}

		public Wish GetWish( int id )
		{
			return _repository.GetWishes().WithId( id );
		}

		public void SaveWish( Wish wish, bool suppressNotifications )
		{
			if (!suppressNotifications && wish.IsCalled && wish.CalledByUser.NotifyOnChange)
			{
				_mailService.SendMail( CreateUpdateNotificationMessage( wish ) );
			}

			_repository.SaveWish( wish );
		}

		public void RemoveWish( Wish wish )
		{
			if (wish.IsCalled && wish.CalledByUser.NotifyOnChange)
			{
				_mailService.SendMail( CreateRemovalNotificationMessage( wish ) );
			}

			_repository.RemoveWish( wish );
		}

		public IList<Wish> GetShoppingList( int userId )
		{
			return _repository.GetWishes().Where( x => x.CalledByUser != null ).Where( x => x.CalledByUser.Id == userId ).ToList();
		}

		public WishList.Data.WishList GetWishList( int ownerId )
		{
			if (_repository.GetUsers().WithId( ownerId ) == null)
			{
				throw new ArgumentException( "User with id " + ownerId + " does not exist - cannot get wish list!" );
			}

			return new WishList.Data.WishList( ownerId,	_repository.GetWishes().ForUser( ownerId ).ToList()	);
		}

		public WishList.Data.WishList GetWishList( string userName )
		{
			User user = _repository.GetUsers().WithName( userName );
			if (user == null)
			{
				throw new ArgumentException( String.Format( "User '{0}' does not exist - cannot get wish list!", userName ) );
			}

			return new WishList.Data.WishList( user.Id, _repository.GetWishes().ForUser( user.Id ).ToList() );
		}

		public IList<Wish> GetLatestActivityList( int userId )
		{
			DateTime limit = DateTime.Now.AddMonths(-1);

			var wishes = (from w in _repository.GetWishes()
						 where w.Changed > limit
						 orderby w.Changed descending
						 select w).Take(10);


			return wishes.ToList();
		}

		private MailMessage CreateRemovalNotificationMessage( Wish wish )
		{
			var mail = new MailMessage(
				"Önskelistemaskinen <noreply@wish.driessen.se>",
				wish.CalledByUser.Email,
				string.Format( "Önskningen \"{0}\" togs bort", wish.Name ),
				string.Format( @"Hej, {0}!

Detta är ett automatiskt meddelande från Önskelistemaskinen. {1} har tagit bort önskningen {2} från sin önskelista (som du hade tjingat).
Du kanske får hitta en annan önskning på önskelistan.

http://wish.driessen.se",
					wish.CalledByUser.Name, wish.Owner.Name, wish.Name )
				);

			return mail;
		}

		private MailMessage CreateUpdateNotificationMessage( Wish wish )
		{
			var mail = new MailMessage(
				"Önskelistemaskinen <noreply@wish.driessen.se>",
				wish.CalledByUser.Email,
				string.Format( "Önskningen \"{0}\" har ändrats", wish.Name ),
				string.Format( @"Hej, {0}!

Detta är ett automatiskt meddelande från Önskelistemaskinen. {1} har ändrat önskningen {2} i sin önskelista (som du har tjingat).
Klicka på nedanstående länk för att kontrollera att inget allvarligt ändrats.

http://wish.driessen.se",
					wish.CalledByUser.Name, wish.Owner.Name, wish.Name )
				);

			return mail;
		}
	}
}
