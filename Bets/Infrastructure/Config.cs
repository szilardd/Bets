using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bets.Helpers
{
	public enum PageMode
	{
		Add = 1,
		Detail = 2,
		List = 3
	}

	public enum Menu
	{
		None = 0,
		Home,
		Master,
		Transaction,
		Setup
	}

	public enum SessionKey
	{
		SubSessionID,
		Breadcrumbs,
		User
	}

	public enum TempKey
	{
		SelectedTab,
		DetailAction
	}

	public enum CacheKey
	{
		PermissionSetDate
	}

	public static class Config
	{
		public const int PageSize = 15;
		public const string ActionStatus = "ActionStatus";
		public static SelectListItem SelectOne = new SelectListItem { Text = "", Value = "" };
	}
}