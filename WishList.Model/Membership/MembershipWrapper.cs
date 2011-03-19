using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WishList.Data.Membership
{
	public class MembershipWrapper : IMembershipWrapper
	{
		public System.Web.Security.MembershipUser GetUser( string username )
		{
			return System.Web.Security.Membership.GetUser( username );
		}

		public System.Web.Security.MembershipUser CreateUser( string username, string password, string email, 
			string passwordQuestion, string passwordAnswer, bool isApproved, out System.Web.Security.MembershipCreateStatus status )
		{
			return System.Web.Security.Membership.CreateUser( username, password, email, passwordQuestion, passwordAnswer, isApproved, out status );
		}

		public void DeleteUser( string username )
		{
			System.Web.Security.Membership.DeleteUser( username );
		}

		public void UpdateUser( System.Web.Security.MembershipUser membershipUser )
		{
			System.Web.Security.Membership.UpdateUser( membershipUser );
		}

		public bool ValidateUser( string username, string password )
		{
			return System.Web.Security.Membership.ValidateUser( username, password );
		}
	}
}
