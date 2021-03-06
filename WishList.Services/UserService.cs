﻿using System;
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
		readonly IWishListRepository wishListRepository;
		readonly IMailService mailService;
		private static readonly string _userListCacheKey = "UserList";

		//public UserService()
		//	: this(new SqlWishListRepository(), new MailService()) { }

		public UserService(IWishListRepository wishListRepository, IMailService mailService)
		{
			this.mailService = mailService;
			this.wishListRepository = wishListRepository;
		}

		public IList<User> GetUsers()
		{
			List<User> userList = new List<User>(UserList.Count);
			foreach (User u in UserList)
			{
				userList.Add(u.Clone());
			}
			return userList;
		}

		public IList<User> GetFriends(User user)
		{
			return wishListRepository.GetFriends(user).ToList();
		}

		public void AddFriend(string username, string friendname)
		{
			var user = GetUser(username);
			var friend = GetUser(friendname);

			wishListRepository.AddFriend(user, friend);
		}

		public void RemoveFriend(string username, string friendname)
		{
			var user = GetUser(username);
			var friend = GetUser(friendname);

			wishListRepository.RemoveFriend(user, friend);
		}

		public User CreateUser(User user)
		{
			try
			{
				User createdUser = wishListRepository.CreateUser(user);
				ClearCache();
				return createdUser;
			}
			catch (InvalidOperationException)
			{
				return null;
			}

		}

		public void DeleteUser(string username)
		{
			throw new NotImplementedException();
		}

		public User GetUser(int userId)
		{
			if (UserList.Any(u => u.Id == userId))
				return UserList.WithId(userId).Clone();
			return null;
		}

		public User GetUser(string username)
		{
			if ((UserList.Any(u => u.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase))))
				return UserList.WithName(username).Clone();
			return null;
		}

		/// <summary>
		/// Log's the user in
		/// </summary>
		public bool ValidateUser(string userName, string password)
		{

			if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				return false;

			return wishListRepository.ValidateUser(userName, password);
		}

		private IList<User> UserList
		{
			get
			{
				IList<User> userList = HttpRuntime.Cache.Get(_userListCacheKey) as IList<User>;
				if (userList == null)
				{
					userList = wishListRepository.GetUsers().ToList<User>();
					HttpRuntime.Cache.Insert(_userListCacheKey, userList);
				}
				return userList;
			}
		}

		public void ClearCache()
		{
			HttpRuntime.Cache.Remove(_userListCacheKey);
		}

		public void ApproveUser(string username, Guid ticket)
		{
			wishListRepository.ApproveUser(username, ticket);
		}

		public Guid? GetApprovalTicket(string username)
		{
			return wishListRepository.GetApprovalTicket(username);
		}

		public User UpdateUser(User user)
		{
			if (string.IsNullOrEmpty(user.Email))
				throw new ArgumentException("Email cannot be empty", "user");

			if (UserList.WithName(user.Name) != null && UserList.WithName(user.Name).Id != user.Id)
				throw new InvalidOperationException(string.Format("Another user with the name {0} already exists!", user.Name));


			User updatedUser = wishListRepository.UpdateUser(user);
			ClearCache();
			return updatedUser;
		}

		public void UpdatePassword(string username, string newPassword)
		{
			if (string.IsNullOrEmpty(newPassword))
			{
				throw new ArgumentException("Password cannot be null or empty", "newPassword");
			}
			if (string.IsNullOrEmpty(username))
			{
				throw new ArgumentException("Username cannot be null or empty", "username");
			}

			wishListRepository.SetPassword(username, newPassword);
			ClearCache();
		}

		public void GenerateNewPassword(int userId)
		{
			var user = GetUser(userId);
			var password = GeneratePassword();

			UpdatePassword(user.Name, password);

			mailService.SendMail(new System.Net.Mail.MailMessage("no-reply@wish.driessen.se", user.Email)
			{
				Subject = "Ditt nya lösenord till Önskelistemaskinen",
				BodyEncoding = System.Text.Encoding.UTF8,
				Body = $"Hej, {user.Name}!\r\n\r\nDitt nya lösenord till Önskelistemaskinen är:\r\n\r\n {password}\r\n\r\nEfter du loggat in kan du byta till vilket lösenord du vill!"
			});
		}

		static string GeneratePassword()
		{
			var words = new List<string>();
			const int length = 3;
			for (var i = 0; i < length; i++)
			{
				var word = Words[random.Next(0, Words.Length - 1)];
				if (words.Contains(word))
				{
					i--;
					continue;
				}
				words.Add(word);
			}
			return string.Join(" ", words);
		}
		readonly static Random random = new Random();
		readonly static string[] Words = new[] { "Advent", "Angels", "Announcement", "Bells", "Bethlehem", "Blitzen", "Candles", "Candy", "Candy canes", "Cards", "Cedar", "Celebrate", "Ceremonies", "Chimney", "Christmas cookies", "Christmas tree", "Cold", "Comet", "Cranberry sauce", "Crowds", "Cupid", "Dancer", "Dasher", "December", "Decorations", "Dolls", "Donner", "Dressing", "Eggnog", "Elves", "Family reunion", "Festival", "Fir", "Frosty", "Fruitcake", "Gift boxes", "Gifts", "Goodwill", "Greetings", "Ham", "Happy", "Holiday", "Holly", "Holy", "Icicles", "Jolly", "Lights", "Lists", "Merry", "Miracle", "Mistletoe", "New Year", "Noel", "North Pole", "Pageant", "Parades", "Party", "Pie", "Pine", "Plum pudding", "Poinsettia", "Prancer", "Presents", "Pumpkin pie", "Punch", "Regreen", "Reindeer", "Ribbon", "Rudolph", "Sacred", "Sales", "Sauce", "Scrooge", "Season", "Sled", "Sleighbells", "Snowflakes", "Spirit", "St. Nick", "Stand", "Star", "Stickers", "Stocking stuffers", "Sweet potato", "Tidings", "Tinsel", "Togetherness", "Toys", "Tradition", "Traffic", "Trips", "Turkey", "Vacation", "Vixen", "Winter", "Worship", "Wrapping paper", "Wreath", "Yule", "Yuletide" };
	}
}
