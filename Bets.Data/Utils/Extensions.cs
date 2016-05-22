using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Routing;
using System.Configuration;
using System.Web.Mvc;
using System.Web;
using System.IO;

namespace Bets.Data
{
	public static class Extensions
	{
        #region "Date"

        public static string ToDate(this object date, bool time = false)
		{
			if (date == null || date.ToString() == "")
				return null;

			return ToDateInternal(Convert.ToDateTime(date), time);
		}

		public static string ToDate(this DateTime? dateTime, bool time = false)
		{
			if (!dateTime.HasValue)
				return null;

			return ToDateInternal(dateTime.Value, time);
		}

		private static string ToDateInternal(this DateTime dateTime, bool time)
		{
			if (dateTime == null)
				return "";

			var easternDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"));
			var format = time ? DataConfig.DateTimeFormat : DataConfig.DateFormat;

			return easternDateTime.ToString(format);
		}

		public static DateTime FromEETToUTC(this DateTime dateTime)
		{
			return TimeZoneInfo.ConvertTimeToUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"));
		}

		public static string ToISODate(this DateTime date, bool includeTime = false)
		{
			return date.ToString("yyyy-MM-dd");
		}

		public static DateTime ToISODate(this object value)
		{
			DateTime date = DateTime.UtcNow;

			DateTime.TryParse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal, out date);

			return date;
		}

		#endregion

		public static string EnumValue<T>(this object value) where T : struct
		{
			T result;
			bool success = Enum.TryParse<T>(value.ToString(), out result);

			if (success && Convert.ToInt32(result) != -1)
				return result.ToString().ToWords();
			else
				return "";
		}

		public static T ToEnum<T>(this object value)
		{
			if (value == null)
				return default(T);

			return (T)Enum.ToObject(typeof(T), value);
		}


		public static T FromJSON<T>(this string JSONObj)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
			return serializer.Deserialize<T>(JSONObj);
		}

		public static string ToJSON(this object obj)
		{
			JavaScriptSerializer serializer = new JavaScriptSerializer { MaxJsonLength = Int32.MaxValue };
			return serializer.Serialize(obj);
		}


		public static int? ToInt(this string value)
		{
			if (String.IsNullOrWhiteSpace(value))
				return null;

			return Convert.ToInt32(value);
		}

		public static RouteValueDictionary ToRouteValue(this Dictionary<string, object> dictionary)
		{
			if (dictionary == null)
				return null;

			return new RouteValueDictionary(dictionary);
		}

		public static string ToWords(this string value)
		{
			return System.Text.RegularExpressions.Regex.Replace(value, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
		}

		public static DateTime GetExpirationDate()
		{
			return DateTime.UtcNow.AddHours(DataConfig.HoursBeforeBet);
		}

        /// <summary>
        /// Until what date and time can a bet be made
        /// </summary>
        /// <param name="betDate"></param>
        /// <returns></returns>
        public static DateTime GetBetCutoffDate(DateTime betDate)
        {
            return (betDate == DateTime.MinValue) ? betDate : betDate.AddHours(-1 * DataConfig.HoursBeforeBet);
        }
    }
}
