using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WishList.Data.Membership;

namespace WishList.IntegrationTests
{
	class MembershipMock : IMembershipWrapper
	{
		private Dictionary<string, System.Web.Security.MembershipUser> users;

		public MembershipMock()
		{
			users = new Dictionary<string, System.Web.Security.MembershipUser>();
		}

		public System.Web.Security.MembershipUser GetUser( string username )
		{
			return users[username];

			//return new System.Web.Security.MembershipUser( "", username, "", "", "", "", true, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue );
		}

		public System.Web.Security.MembershipUser CreateUser( string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, out System.Web.Security.MembershipCreateStatus status )
		{
			if (users.ContainsKey( username ))
				throw new InvalidOperationException();

			status = System.Web.Security.MembershipCreateStatus.Success;
			var user = new System.Web.Security.MembershipUser( "AspNetSqlMembershipProvider", username, username, email, "", "", isApproved, false, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue );
			users.Add( username, user );
			return user;
		}

		public void DeleteUser( string username )
		{
			users.Remove( username );
		}

		public void UpdateUser( System.Web.Security.MembershipUser membershipUser )
		{
			users[membershipUser.UserName] = membershipUser;
		}

		public bool ValidateUser( string username, string password )
		{
			return users[username] != null && users[username].IsApproved;
		}
	}
}
