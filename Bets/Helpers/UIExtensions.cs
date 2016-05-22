using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Bets.Data;
using Bets.Helpers;

namespace Bets.Helpers
{
	public static class UIExtensions
	{

		public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> array, object selectedValue = null, SelectListItem emptyOption = null)
		{
			List<SelectListItem> selectList = new List<SelectListItem>();
			DropdownOption option;
			string text, value, selectedValueText = (selectedValue ?? "").ToString();
			bool selected;

			if (emptyOption != null)
				selectList.Add(emptyOption);

			foreach (object item in array)
			{
				if (item.GetType() == typeof(DropdownOption))
					option = (DropdownOption)item;
				else
					option = new DropdownOption { Text = item.ToString(), Value = item.ToString() };

				text = (option.Text ?? "").ToString();
				value = (option.Value ?? "").ToString();

				selected = selectedValueText != null && (value == selectedValueText);

				selectList.Add(new SelectListItem { Text = text, Value = value, Selected = selected });
			}

			return selectList;
		}

		public static List<SelectListItem> ToSelectList<T>(this IEnumerable<T> array, object selectedValue = null, bool addEmptyOption = true)
		{
			SelectListItem emptyOption = null;

			if (addEmptyOption)
				emptyOption = Config.SelectOne;

			return ToSelectList<T>(array, selectedValue, emptyOption);
			
		}
	}
}