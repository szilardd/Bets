using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Bets.Data.Models;

namespace ThatAuthentication
{
	[Serializable]
	public class ThatMembershipUser : MembershipUser
	{
		public Role Role { get; set; }
		public string DisplayName { get; set; }

		public static int UserID
		{
			get
			{
				return Convert.ToInt32(Membership.GetUser().ProviderUserKey);
			}
		}

		public ThatMembershipUser(string providerName, string name, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate, Role role, string displayName)
		   : base(providerName, name, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
		{
			this.Role = role;
			this.DisplayName = displayName;
		}
	}
}