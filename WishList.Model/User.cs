using System;
using System.Collections.Generic;
using System.Text;

namespace WishList.Data
{
	public class User
	{

		//public User( string name, string email, string password )
		//{
		//	Name = name;
		//	Email = email;
		//	Password = password;
		//}

		//public User( int id, string name, string email, string password )
		//{
		//    Id = id;
		//    Name = name;
		//    Email = email;
		//    Password = password;
		//}

		public int Id
		{
			get;
			set;
		}

		public string Email
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool NotifyOnChange
		{
			get;
			set;
		}

		internal string Password
		{
			set;
			get;
		}

		public void SetPassword( string password )
		{
			Password = password;
		}

		//public bool CheckPassword( string password )
		//{
		//    return password == Password;
		//}

		public User Clone()
		{
			return new User()
			{
				Name = this.Name,
				Email = this.Email,
				Password = this.Password,
				Id = this.Id,
				NotifyOnChange = this.NotifyOnChange
			};
		}
	}
}
