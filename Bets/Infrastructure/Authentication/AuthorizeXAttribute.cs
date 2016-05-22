using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using System.Web.Security;
using Bets.Data.Models;

namespace ThatAuthentication
{
	public class AuthorizeXAttribute : AuthorizeAttribute
	{
		private Role allowedRole;

		public AuthorizeXAttribute(Role role)
		{
			this.allowedRole = role;
		}

		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext == null)
				throw new ArgumentNullException("httpContext");

			string[] users = Users.Split(',');

			if (!httpContext.User.Identity.IsAuthenticated)
				return false;

			var user = Membership.GetUser() as ThatMembershipUser;

			if (user == null || (allowedRole != 0 && allowedRole > user.Role))
				return false;

			return true;
		}
	}
}