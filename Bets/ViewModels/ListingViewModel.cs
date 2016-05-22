using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bets.Data.Models;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bets.Models
{
	/// <summary>
	/// View model passed to listing pages
	/// </summary>
	public class ListingViewModel
	{
		public bool IsSubPage { get; set; }
		public bool IsSubDetailPage { get; set; }

		//true if the listing models a lookup page
		public bool IsLookup { get; set; }

		//true if the page has advanced filters
		public bool HasAdvancedFilter { get; set; }

		//true if the page should show advanced date filter
		public bool HasDateFilter { get; set; }

		//true if the item should link to a detail page in the listing
		public bool HasDetailPage { get; set; }

		//true if the page is searchable
		public bool HasSearch { get; set; }

		//true if adding a new item is done through a lookup page
		public bool HasLookup { get; set; }

		public bool HasPageNumbers { get; set; }

		//true if new item can be added
		public bool AllowNew { get; set; }

		//true if delete should be allowed
		public bool AllowDelete { get; set; }

		//true if the advanced fiters should be shown by default on page load
		public bool IsAdvancedFilterOn { get; set; }

		//active saved search
		public int SearchID { get; set; }

		//pagination index
		public int? PageIndex { get; set; }

		//how many records are displayed in the listing
		public int ItemsPerPage { get; set; }

		public int? MinPageSize { get; set; }

		public IModel Model { get; set; }
		public Module Module { get; set; }
		public Module ParentModule { get; set; }
		public PagedList<IModel> Items { get; set; }

		//advanced date filter types (Today, Tomorrow, Next Week, etc.)
		public IEnumerable<SelectListItem> DateFilter { get; set; }

		//adavanced filters to be displayed in the listing
		public Dictionary<string, IEnumerable<SelectListItem>> AdvancedFilters { get; set; }

		//current values for the advanced filters
		public Dictionary<string, object> AdvancedFilterValues { get; set; }
		
		//dependecy relationships between advanced filter; causes dependent value loading (e.g: Customer -> TypeID, GroupID, AccountID)
		public Dictionary<string, string> AdvancedFilterDependencies { get; set; }

		//
		public Dictionary<string, object> Parameters { get; set; }

		//current search keyword
		public string Keyword { get; set; }

		//master page layout name
		public string Layout { get; set; }

		//type of page (eg.: FollowUp)
		public string PageType { get; set; }

		//subtype of page (e.g. SalesCallFollowUp)
		public string SubPageType { get; set; }

		//display label based on page type and name
		public string Label { get; set; }

		//view template name
		public string TemplateName { get; set; }

		public string ItemTemplateName { get; set; }

		public ListingViewModel()
		{
			this.HasPageNumbers = false;
			this.HasSearch = false;
			this.HasDetailPage = false;
			this.AllowNew = true;
			this.AllowDelete = true;
			this.IsAdvancedFilterOn = false;
			this.TemplateName = "";
		}

		public string GetAdvancedFilterValue(string name)
		{
			if (this.AdvancedFilterValues == null || !this.AdvancedFilterValues.ContainsKey(name))
				return "";

			return this.AdvancedFilterValues[name].ToString();
		}
	}
}