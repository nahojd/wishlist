using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishList.Data.DataAccess;
using WishList.Data;
using WishList.Data.Filters;
using System.Reflection;

namespace WishList.Tests
{
	/// <summary>
	/// This is a hard-coded repository that resides only in RAM
	/// It intially contains 5 users with names "User 1" to "User 5" and passwords "pwd1" to "pwd5"
	/// User 1 has 5 wishes.
	/// </summary>
	public class TestWishListRepository : IWishListRepository
	{
		private IList<Wish> _wishes;
		private IList<User> _users;

		public TestWishListRepository()
		{
			//Create some users
			_users = new List<User>();
			_wishes = new List<Wish>();

			for (int userId = 1; userId <= 5; userId++)
			{
				_users.Add(
					new User { Id = userId, Name = "User " + userId, Email = "user" + userId + "@example.com", Password = "pwd" + userId }
					);

				//Create some wishes for each user
				for (int wishNumber = 1; wishNumber <= 5; wishNumber++)
				{
					Wish wish = new Wish() { Id = _wishes.Count + 1, Name = "Wish" + wishNumber, Description = "Description " + wishNumber, Owner = _users.WithId( userId ), Created = DateTime.Now, Changed = DateTime.Now };

					//Call some wishes for user 1 - but not his own wishes!;
					if (userId != 1 && wishNumber == 1)
					{
						wish.CalledByUser = _users.WithId( 1 );
					}
					_wishes.Add( wish );
				}
			}

			//And a special one for updating tests
			_users.Add( new User { Id = 17, Name = "UpdateUser", Email = "updateuser@example.com", Password = "update" } );

		}

		#region IWishListRepository Members

		public IQueryable<User> GetUsers()
		{
			return _users.AsQueryable<User>();
		}

		public IQueryable<WishList.Data.Wish> GetWishes()
		{
			List<Wish> wishesToReturn = new List<Wish>();
			foreach (var wish in _wishes)
			{
				wishesToReturn.Add( wish.Clone() as Wish );
			}
			return wishesToReturn.AsQueryable<Wish>();
		}

		public Wish SaveWish( Wish wish )
		{
			if (wish.Id < 1)
			{
				wish.Id = _wishes.Count + 1;
				wish.Created = DateTime.Now;
				wish.Changed = wish.Created;
				_wishes.Add( wish );
				return wish;
			}
			else
			{
				Wish wishToUpdate = (from w in _wishes where w.Id == wish.Id select w).SingleOrDefault<Wish>();
				if (wishToUpdate != null)
				{
					//Don't update change date if the wish was called

					if (!wish.CalledDiffers( wishToUpdate.CalledByUser != null ? (int?)wishToUpdate.CalledByUser.Id : null ))
					{
						wishToUpdate.Changed = DateTime.Now;
					}

					wishToUpdate.Description = wish.Description;
					wishToUpdate.LinkUrl = wish.LinkUrl;
					wishToUpdate.Name = wish.Name;
					wishToUpdate.Owner = wish.Owner;
					wishToUpdate.CalledByUser = wish.CalledByUser;

					return wishToUpdate;
				}
				else
				{
					throw new InvalidOperationException( "Could not update wish - did not exist in repository" );
				}
			}
		}

		public void RemoveWish( Wish wish )
		{
			var wishToBeRemoved = (from w in _wishes where w.Id == wish.Id select w).SingleOrDefault<Wish>();
			if (wishToBeRemoved != null)
				_wishes.Remove( wishToBeRemoved );
		}

		public User CreateUser( User user )
		{
			bool exists = (from u in _users where u.Name == user.Name select u).Count<User>() > 0;
			if (exists)
			{
				throw new InvalidOperationException( "Duplicate username" );
			}

			User newUser = user.Clone();
			newUser.Id = _users.Count + 1;
			_users.Add( newUser );

			return newUser;
		}

		public bool ValidateUser( string userName, string password )
		{
			User user = (from u in _users where u.Name == userName select u).SingleOrDefault();

			if (user == null)
			{
				return false;
			}

			return GetUserPassword( user ) == password;

		}

		private string GetUserPassword( User user )
		{
			PropertyInfo prop = typeof( User ).GetProperty( "Password", BindingFlags.Instance | BindingFlags.NonPublic );
			return prop.GetValue( user, null ) as string;

		}

		public void ApproveUser( string username, Guid ticket )
		{
			//Nothing to do here
		}

		public Guid? GetApprovalTicket( string username )
		{
			return Guid.NewGuid();
		}

		public User UpdateUser( User user )
		{
			User internalUser = _users.WithId( user.Id );
			internalUser.Email = user.Email;
			internalUser.NotifyOnChange = user.NotifyOnChange;

			return internalUser;
		}


		public void SetPassword( string username, string password )
		{
			User user = _users.WithName( username );
			user.SetPassword( password );
		}

		#endregion
	}
}
