using System;
using System.Linq;
using System.Web.Security;
using WishList.SqlRepository;
using RepData = WishList.SqlRepository.Data;
using System.Reflection;

namespace WishList.Data.DataAccess
{
	public class SqlWishListRepository : IWishListRepository, IDisposable
	{
		SqlRepository.LinqWishListDataContext readDataContext;
		LinqWishListDataContext writeDataContext;

		bool disposed;

		public SqlWishListRepository()
		{
			readDataContext = new SqlRepository.LinqWishListDataContext { ObjectTrackingEnabled = false };
		}

		public SqlWishListRepository( LinqWishListDataContext dataContext )
		{
			readDataContext = dataContext;
			writeDataContext = dataContext;
		}

		~SqlWishListRepository()
		{
			this.Dispose();
		}

		public void Dispose()
		{
			if (!disposed)
			{
				readDataContext.Dispose();
				if (writeDataContext != null)
				{
					writeDataContext.Dispose();
				}
				disposed = true;
			}
		}

		public IQueryable<User> GetUsers()
		{
			return from user in readDataContext.Users
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
			return from wish in readDataContext.Wishes
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
			var dataContext = GetWriteDataContext();
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

		public void RemoveWish( Wish wish )
		{

			var dataContext = GetWriteDataContext();
			RepData.Wish wishToBeRemoved = (from w in dataContext.Wishes
											where w.WishId == wish.Id
											select w).SingleOrDefault();

			dataContext.Wishes.DeleteOnSubmit( wishToBeRemoved );
			dataContext.SubmitChanges();
		}

		public User CreateUser( User user )
		{
			//MembershipCreateStatus createStatus;
			//MembershipUser membershipUser = membership.CreateUser( user.Name, user.Password, user.Email, null, null, false, out createStatus );

			//if (createStatus == MembershipCreateStatus.Success)
			//{

			if (!UsernameIsUnique( user.Name ))
				throw new InvalidOperationException( "A user with that name already exists!" );


			var salt = CreateSalt( user );
			var passwordHash = GetHash( user.Password, salt );

			var newUser = new RepData.User
							{
								Name = user.Name,
								Email = user.Email,
								NotifyOnChange = user.NotifyOnChange,
								ApprovalTicket = Guid.NewGuid(),
								PasswordHash = passwordHash,
								Salt = salt
							};

			//try
			//{
			var dataContext = GetWriteDataContext();
			dataContext.Users.InsertOnSubmit( newUser );
			dataContext.SubmitChanges();


			var createdUser = user.Clone();
			createdUser.Id = newUser.UserId;
			return createdUser;
			//}
			//    catch (Exception)
			//    {
			//        membership.DeleteUser( membershipUser.UserName );
			//        throw;
			//    }
			//}

			//throw new InvalidOperationException( "Could not register user: " + createStatus );
		}

		public bool UsernameIsUnique( string username )
		{
			return !GetUsers().Any( u => u.Name.ToLower() == username.ToLower() );
		}

		internal static string CreateSalt( User user )
		{
			return GetHash( user.Name + DateTime.Now.Ticks, "" );
		}

		internal static string GetHash( string password, string salt )
		{
			//TODO: Remove dependency of SqlMembershipProvider without changing behavior...
			var type = typeof( SqlMembershipProvider );
			BindingFlags privateBindings = BindingFlags.NonPublic | BindingFlags.Instance;
			MethodInfo miGetDescription = type.GetMethod( "EncodePassword", privateBindings );
			var provider = new SqlMembershipProvider();
			var hash = miGetDescription.Invoke( provider, new object[] { password, 1, salt } ) as string;
			return hash;
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

			//MembershipUser mu = membership.GetUser( username );
			//if (mu == null)
			//{
			//    throw new InvalidOperationException( String.Format( "No user with username {0}", username ) );
			//}

			//mu.IsApproved = true;
			//membership.UpdateUser( mu );

			var dataContext = GetWriteDataContext();
			var user = (from u in dataContext.Users
						where u.Name == username
						select u).SingleOrDefault();
			user.ApprovalTicket = null;
			dataContext.SubmitChanges();
		}

		public bool ValidateUser( string username, string password )
		{
			var user = readDataContext.Users
				.Where( u => u.Name == username )
				.Where( u => u.ApprovalTicket == null )
				.SingleOrDefault();

			return user != null && user.PasswordHash == GetHash( password, user.Salt );
		}

		public Guid? GetApprovalTicket( string username )
		{
			return (from u in readDataContext.Users
					where u.Name == username
					select u.ApprovalTicket).SingleOrDefault();
		}

		public User UpdateUser( User user )
		{
			var dataContext = GetWriteDataContext();
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

		public void SetPassword( string username, string password )
		{
			var dataContext = GetWriteDataContext();
			var user = dataContext.Users.Where( u => u.Name == username ).SingleOrDefault();
			if (user == null)
				throw new ArgumentException( "No such user", "username" );
			user.PasswordHash = GetHash( password, user.Salt );
			dataContext.SubmitChanges();
		}

		private static string TruncateString( string str, int length )
		{
			if (str == null)
				return null;

			return str.Length > length ? str.Substring( 0, length ) : str;
		}


		public IQueryable<User> GetFriends( User user )
		{
			return from u in readDataContext.Users
				   where readDataContext.Friends.Any( x => x.UserId == user.Id && x.FriendId == u.UserId )
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
			var context = GetWriteDataContext();
			var friendAlreadyExists = GetFriend( user, friend, context ) != null;
			if (friendAlreadyExists)
				throw new InvalidOperationException( String.Format( "{0} är redan vän till {1}!", friend.Name, user.Name ) );

			context.Friends.InsertOnSubmit( new RepData.Friend { UserId = user.Id, FriendId = friend.Id } );
			context.SubmitChanges();
		}

		public void RemoveFriend( User user, User friend )
		{
			var context = GetWriteDataContext();
			var friendToRemove = GetFriend( user, friend, context );

			context.Friends.DeleteOnSubmit( friendToRemove );
			context.SubmitChanges();
		}

		private static RepData.Friend GetFriend( User user, User friend, LinqWishListDataContext context )
		{
			return (from f in context.Friends
					where f.UserId == user.Id && f.FriendId == friend.Id
					select f).SingleOrDefault();
		}

		private LinqWishListDataContext GetWriteDataContext()
		{
			if (writeDataContext == null)
				writeDataContext = new LinqWishListDataContext();
			return writeDataContext;
		}
	}
}
