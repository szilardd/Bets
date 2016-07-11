using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.Web.Security;

namespace Bets.Helpers
{
	public static class DataExtensions
	{
		/// <summary>Converts json from value provider into dictionary</summary>
		public static Dictionary<string, object> JSONToDictionary(ValueProviderResult result)
		{
			if (result == null)
				return null;

			return JSONToDictionary(result.AttemptedValue);
		}

		/// <summary>Converts json string into dictionary</summary>
		public static Dictionary<string, object> JSONToDictionary(string json)
		{
			if (json == null)
				return null;

			Dictionary<string, object> dict = null;

			if (json != null)
				dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(json);

			return dict;
		}

		public static bool UserIsAdmin()
		{
			var user = Membership.GetUser() as ThatAuthentication.ThatMembershipUser;
			return (user.Role == Data.Models.Role.Admin);
		}
    }
}