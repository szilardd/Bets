using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bets.Data.Models
{
	public class ListingParams<M> where M : Model
	{
		public int? PageIndex { get; set; }
		public int ItemsPerPage { get; set; }
		public string Keyword { get; set; }
		public M Model { get; set; }
		public ListingSortModel Sort { get; set; }
		public Dictionary<string, object> AdvancedFilters { get; set; }
		public string Origin { get; set; }
	}
}
