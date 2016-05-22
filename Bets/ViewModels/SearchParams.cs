using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Bets.Models
{
	[Serializable]
	public class SearchParams
	{
		public string SessionPrefix { get; set; }

		public string Keyword { get; set; }
		public int? PageIndex { get; set; }
		public int? ItemsPerPage { get; set; }
		public string SortByColumn { get; set; }
		public bool? SortAsc { get; set; }
		public string AdvancedFilters { get; set; }

		public SearchParams()
		{

		}

		public SearchParams(SerializationInfo info, StreamingContext context)
		{
			Keyword = (string)info.GetValue("Keyword", typeof(string));
			PageIndex = (int?)info.GetValue("PageIndex", typeof(int?));
			ItemsPerPage = (int?)info.GetValue("ItemsPerPage", typeof(int?));
			SortByColumn = (string)info.GetValue("SortByColumn", typeof(string));
			SortAsc = (bool?)info.GetValue("SortAsc", typeof(bool?));
			AdvancedFilters = (string)info.GetValue("AdvancedFilters", typeof(string));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Keyword", Keyword);
			info.AddValue("PageIndex", PageIndex);
			info.AddValue("ItemsPerPage", ItemsPerPage);
			info.AddValue("SortByColumn", SortByColumn);
			info.AddValue("SortAsc", SortAsc);
			info.AddValue("AdvancedFilters", AdvancedFilters);
		}
	}
}