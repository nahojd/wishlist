using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

namespace WishList.Data.Membership
{
	public interface IMembershipWrapper
	{
		MembershipUser GetUser( string username );

		MembershipUser CreateUser( string username, string password, string email,
			string passwordQuestion, string passwordAnswer, bool isApproved, out System.Web.Security.MembershipCreateStatus status );

		void DeleteUser( string username );

		void UpdateUser( MembershipUser membershipUser );

		bool ValidateUser( string username, string password );
	}
}
