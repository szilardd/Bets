using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ThatAuthentication;
using System.Text;
using Bets.Data;
using Bets.Helpers;
using System.Xml.Linq;
using System.Web.Mvc.Html;

namespace Bets.Helpers
{
	public static class HtmlHelpers
	{
		public static string EnumValue<T>(this HtmlHelper helper, object value) where T : struct
		{
			T result;
			bool success = Enum.TryParse<T>(value.ToString(), out result);

			if (success && Convert.ToInt32(result) != -1 )
				return result.ToString();
			else
				return "";
		}

		public static string Date(this HtmlHelper helper, DateTime? value)
		{
			return value == null ? "" : value.Value.ToShortDateString();
		}

		public static T ToEnum<T>(this HtmlHelper helper, object value)
		{
			return (T)Enum.ToObject(typeof(T), value);
		}

		public static bool IsTruthy(this HtmlHelper helper, object value)
		{
			string val = value.ToString().ToLower();

			return val == "1" || val == "true";
		}

		public static bool ViewExists(this HtmlHelper helper, string name)
		{
			ViewEngineResult result = ViewEngines.Engines.FindView(helper.ViewContext.Controller.ControllerContext, name, null);
			return (result.View != null);
		}

		public static MvcHtmlString GetOptions(this HtmlHelper helper, IEnumerable<SelectListItem> options)
		{
			var content = new StringBuilder();

			foreach (var option in options)
				content.Append(String.Format("<option value='{0}'>{1}</option>", option.Value, option.Text));

			return new MvcHtmlString(content.ToString());
		}

		/// <summary>
		/// Builds dropdown by adding data-custom attribute and value to all options
		/// </summary>
		public static MvcHtmlString CustomDataDropdown(this HtmlHelper html, string name, bool addEmpty = false, Dictionary<string, object> htmlAttributes = null)
		{
			var selectList = (List<DropdownOption>)html.ViewData[name];

			if (addEmpty)
				selectList.Add(new DropdownOption { Value = "", Text = ""});

			object selectedValue = selectList.Where(e => e.Selected).Select(e => e.Value).FirstOrDefault();

			var values = (IEnumerable<DropdownOption>)selectList;
			var selectDoc = XDocument.Parse(html.DropDownList(name, values, htmlAttributes).ToString());

			var options = from XElement el in selectDoc.Element("select").Descendants()
						  select el;

			for (int i = 0; i < options.Count(); i++)
			{
				var option = options.ElementAt(i);
				var customOption = selectList.ElementAt(i) as DropdownOption;

				if (customOption.CustomData != null || customOption.CustomData2 != null)
					option.SetAttributeValue("data-custom", (customOption.CustomData ?? "").ToString());

				if (customOption.CustomData2 != null)
					option.SetAttributeValue("data-custom2", customOption.CustomData2.ToString());

				option.SetAttributeValue("value", customOption.Value);

				if (customOption.Value.ToString() == (selectedValue ?? "").ToString())
					option.SetAttributeValue("selected", "selected");
			}

			// rebuild the control, resetting the options with the ones modified
			selectDoc.Root.ReplaceNodes(options.ToArray());
			return MvcHtmlString.Create(selectDoc.ToString());
		}
	}
}