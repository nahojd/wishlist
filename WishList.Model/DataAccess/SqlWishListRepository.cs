using System;
using System.Linq;
using System.Web.Security;
using WishList.SqlRepository;
using RepData = WishList.SqlRepository.Data;

namespace WishList.Data.DataAccess
{
	public class SqlWishListRepository : IWishListRepository
	{
		SqlRepository.LinqWishListDataContext db;

		public SqlWishListRepository()
		{
			db = new SqlRepository.LinqWishListDataContext { ObjectTrackingEnabled = false };
		}

		~SqlWishListRepository()
		{
			db.Dispose();
		}

		public IQueryable<User> GetUsers()
		{
			return from user in db.Users
				   select new User
				   {
					   Id = user.UserId,
					   Name = user.Name,
					   Email = user.Email,
					   NotifyOnChange = user.NotifyOnChange
				   };
		}

		public IQueryable<Wish> GetWishes()
		{
			return from wish in db.Wishes
				   select new Wish
				   {
					   Id = wish.WishId,
					   Name = wish.Name,
					   Description = wish.Description,
					   LinkUrl = wish.LinkUrl,
					   Changed = wish.Changed,
					   Created = wish.Created,
					   Owner = GetUsers().Where( u => u.Id == wish.OwnerId ).SingleOrDefault(),
					   CalledByUser = GetUsers().Where( u => u.Id == wish.TjingedById ).SingleOrDefault()
				   };
		}

		/// <summary>
		/// Saves the wish.
		/// </summary>
		/// <param name="wish">The saved wish.</param>
		/// <returns></returns>
		public Wish SaveWish( Wish wish )
		{
			using (var dataContext = new SqlRepository.LinqWishListDataContext())
			{
				wish.Name = TruncateString( wish.Name, 100 );
				wish.Description = TruncateString( wish.Description, 500 );
				wish.LinkUrl = TruncateString( wish.LinkUrl, 255 );

				DateTime timeStamp = DateTime.Now;

				var wishToSave = (from w in dataContext.Wishes
								  where w.WishId == wish.Id
								  select w).SingleOrDefault();

				bool createNew = wishToSave == null;
				if (createNew)
				{
					wishToSave = new RepData.Wish();
					dataContext.Wishes.InsertOnSubmit( wishToSave );
					wishToSave.Created = timeStamp;
					wishToSave.Changed = timeStamp;
				}
				else if (!wish.CalledDiffers( wishToSave.TjingedById ))
				{ //We don't want to update the change time if the wish was called or uncalled
					wishToSave.Changed = timeStamp;
				}

				wishToSave.Name = wish.Name;
				wishToSave.Description = wish.Description;
				wishToSave.LinkUrl = wish.LinkUrl;
				wishToSave.TjingedById = wish.CalledByUser != null ? (int?)wish.CalledByUser.Id : null;
				wishToSave.OwnerId = wish.Owner.Id;

				dataContext.SubmitChanges();

				Wish savedWish = (Wish)wish.Clone();
				savedWish.Id = wishToSave.WishId;
				if (createNew)
				{
					savedWish.Created = wishToSave.Created;
				}
				savedWish.Changed = wishToSave.Changed;
				return savedWish;
			}
		}

		public void RemoveWish( Wish wish )
		{
			using (var dataContext = new SqlRepository.LinqWishListDataContext())
			{
				RepData.Wish wishToBeRemoved = (from w in dataContext.Wishes
											  where w.WishId == wish.Id
											  select w).SingleOrDefault();

				dataContext.Wishes.DeleteOnSubmit( wishToBeRemoved );
				dataContext.SubmitChanges();
			}
		}

		public User CreateUser( User user )
		{
			MembershipCreateStatus createStatus;
			MembershipUser membershipUser = Membership.CreateUser( user.Name, user.Password, user.Email, null, null, false, out createStatus );

			if (createStatus == MembershipCreateStatus.Success)
			{
				var newUser = new RepData.User
								{
									Name = user.Name,
									Email = user.Email,
									NotifyOnChange = user.NotifyOnChange,
									ApprovalTicket = Guid.NewGuid()
								};

				try
				{
					using (var dataContext = new SqlRepository.LinqWishListDataContext())
					{
						dataContext.Users.InsertOnSubmit( newUser );
						dataContext.SubmitChanges();
					}

					User createdUser = user.Clone();
					createdUser.Id = newUser.UserId;
					return createdUser;
				}
				catch (Exception)
				{
					Membership.DeleteUser( membershipUser.UserName );
					throw;
				}
			}

			throw new InvalidOperationException( "Could not register user: " + createStatus );
		}

		public void ApproveUser( string username, Guid ticket )
		{
			if (string.IsNullOrEmpty( username ))
			{
				throw new ArgumentException( "username was null or empty" );
			}

			Guid? correctTicket = GetApprovalTicket( username );
			if (!correctTicket.HasValue || ticket != correctTicket.Value)
			{
				throw new InvalidOperationException( "Wrong ticket" );
			}

			MembershipUser mu = Membership.GetUser( username );
			if (mu == null)
			{
				throw new InvalidOperationException( String.Format( "No user with username {0}", username ) );
			}

			mu.IsApproved = true;
			Membership.UpdateUser( mu );

			using (var dataContext = new SqlRepository.LinqWishListDataContext())
			{
				var user = (from u in dataContext.Users
							where u.Name == username
							select u).SingleOrDefault();
				user.ApprovalTicket = null;
				dataContext.SubmitChanges();
			}
		}

		public bool ValidateUser( string username, string password )
		{
			return Membership.ValidateUser( username, password );
		}

		public Guid? GetApprovalTicket( string username )
		{
			return (from u in db.Users
					where u.Name == username
					select u.ApprovalTicket).SingleOrDefault();
		}

		public User UpdateUser( User user )
		{
			using (var dataContext = new SqlRepository.LinqWishListDataContext())
			{
				var dbUser = (from u in dataContext.Users
							  where u.UserId == user.Id
							  select u).SingleOrDefault();
				dbUser.Email = user.Email;
				dbUser.NotifyOnChange = user.NotifyOnChange;
				dataContext.SubmitChanges();

				return new User
				{
					Id = dbUser.UserId,
					Name = dbUser.Name,
					Email = dbUser.Email,
					NotifyOnChange = dbUser.NotifyOnChange
				};
			}
		}

		public void SetPassword( string username, string password )
		{

			MembershipUser mu = Membership.GetUser( username );
			mu.ChangePassword( mu.ResetPassword(), password );

		}

		private static string TruncateString( string str, int length )
		{
			if (str == null)
				return null;

			return str.Length > length ? str.Substring( 0, length ) : str;
		}


		public IQueryable<User> GetFriends( User user )
		{
			return from u in db.Users
				   where db.Friends.Any( x => x.UserId == user.Id && x.FriendId == u.UserId )
				   select new User
					{
						Id = u.UserId,
						Name = u.Name,
						Email = u.Email,
						NotifyOnChange = u.NotifyOnChange
					};
		}

		public void AddFriend( User user, User friend )
		{
			using (var context = new LinqWishListDataContext())
			{
				var friendAlreadyExists = GetFriend( user, friend, context ) != null;
				if (friendAlreadyExists)
					throw new InvalidOperationException( String.Format( "{0} är redan vän till {1}!", friend.Name, user.Name ) );

				context.Friends.InsertOnSubmit( new RepData.Friend { UserId = user.Id, FriendId = friend.Id } );
				context.SubmitChanges();
			}
		}

		public void RemoveFriend( User user, User friend )
		{
			using (var context = new LinqWishListDataContext())
			{
				var friendToRemove = GetFriend( user, friend, context );

				context.Friends.DeleteOnSubmit( friendToRemove );
				context.SubmitChanges();
			}
		}

		private static RepData.Friend GetFriend( User user, User friend, LinqWishListDataContext context )
		{
			return (from f in context.Friends
					where f.UserId == user.Id && f.FriendId == friend.Id
					select f).SingleOrDefault();
		}
	}
}
