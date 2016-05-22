using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.Helpers;
using System.Web.Routing;
using Bets.Data;
using Bets.Data.Models;
using Bets.Helpers;
using Bets.Models;
using ThatAuthentication;
using System.Dynamic;

namespace Bets.Controllers
{
	/// <summary>
	/// Base calss which provides CRUD functionality for listing, detail and add pages. The controller is identified by three parameters:
	/// - model (inehrits from Model and implements IModel interface)
	/// - entity (dabase entity from dbml)
	/// - repository (inherits from the generic Repository class, identified by the model and entity)
	/// </summary>
	/// <typeparam name="M"></typeparam>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="R"></typeparam>
	public class CRUDController<M, T, R> : Controller		where M : Model, IModel, new()
															where T : class
															where R : Repository<T, M>, new()
	{

		#region "Internal"

		public IModel DetailModel { get; set; }
		protected Menu parentMenu;
		public PageMode PageMode { get; set; }
		protected R repo = new R();
		private string pageType, name;
		private bool isSubDetailPage;
		private Module module;
		private string searchParamsSessionKey;

		/// <summary>
		/// 
		/// </summary>
		protected bool IsSubDetailPage
		{
			get { return isSubDetailPage; }
			set { isSubDetailPage = value; this.ListingViewModel.IsSubDetailPage = value; }
		}

		/// <summary>
		/// Flag which determines whether the page is a lookup page
		/// </summary>
		protected bool IsLookup { get; set; }

		/// <summary>
		/// Identified main menu (in the main menu of the site)
		/// </summary>
		protected Menu ParentMenu
		{
			get { return parentMenu; }
			set
			{
				parentMenu = value;
				this.ListingViewModel.Layout = value.ToString();
			}
		}

		/// <summary>
		/// Identified the  submenu (in the sidebar of the page)
		/// </summary>
		protected Module SubMenu { get; set; }

		protected ListingViewModel ListingViewModel { get; set; }
		protected DetailViewModel DetailViewModel { get; set; }

		/// <summary>
		/// Pages are assigned to modules. A page can be accessed only if the logged in user has permission assigned to its module
		/// </summary>
		protected Module Module
		{
			get { return module; }
			set
			{
				module = value;
				this.ListingViewModel.Module = module;
			}
		}

		/// <summary>
		/// Unique name of the page
		/// </summary>
		protected string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				pageType = value;
				this.ListingViewModel.Label = name.ToWords();
				this.ListingViewModel.PageType = value;
			}
		}

		/// <summary>
		/// Page type, which uniquely identifies parent and sub-pages (e.g: FollowUp and SalesCallFollowUp)
		/// </summary>
		protected string PageType
		{
			get
			{
				return pageType;
			}
			set
			{
				pageType = (value == null) ? this.Name : value;
				this.ListingViewModel.SubPageType = pageType;
				this.searchParamsSessionKey = this.pageType.ToLower() + "_SearchParams";
			}
		}

		protected enum ViewType
		{
			Index,
			Detail,
			List
		}

		public CRUDController()
		{
			var user = Membership.GetUser() as ThatMembershipUser;

            if (user != null)
            {
                repo.UserID = Convert.ToInt32(user.ProviderUserKey);
                repo.IsAdmin = (user.Role == Role.Admin);
            }

			this.ListingViewModel = new ListingViewModel();
		}

		/// <summary>
		/// Returns name of the view to be loaded based on the viewtype
		/// </summary>
		/// <param name="pageType"></param>
		/// <returns></returns>
		protected string GetViewName(ViewType pageType)
		{
			string name = "";

			switch (pageType)
			{
				case ViewType.Index: name = this.IsLookup ? Url.Template("Lookup") : Url.Layout("ListingPage", false); break;
				case ViewType.Detail: name = this.pageType + "Detail"; break;
				case ViewType.List: name = this.IsLookup ? Url.Template("LookupList") : Url.Layout(this.ListingViewModel.TemplateName + "ListingData"); break;
			}

			return name;
		}

		/// <summary>
		/// Set tab name to be selected by default when the page is shown (on detail pages)
		/// </summary>
		/// <param name="tab"></param>
        /// <param name="action">Action type</param>
		protected void SetSelectedDetailTab(string tab, DetailAction action)
		{
			TempData[TempKey.SelectedTab.ToString()] = tab;
			TempData[TempKey.DetailAction.ToString()] = action;
		}

		private void CopyDataModelToViewModel(ListingParams<M> listingDataModel)
		{
			this.ListingViewModel.AdvancedFilterValues = listingDataModel.AdvancedFilters;
			this.ListingViewModel.Keyword = listingDataModel.Keyword;
		}

		private void CopyViewModelToDataModel(ListingParams<M> listingDataModel)
		{
			listingDataModel.AdvancedFilters = this.ListingViewModel.AdvancedFilterValues;
			listingDataModel.Keyword = this.ListingViewModel.Keyword;
		}

		/// <summary>
		/// Sets listing data (search keyword, sorting, advanced filters, pagination, saved search)
		/// </summary>
		/// <param name="listingDataModel"></param>
		/// <param name="forIndex"></param>
		private void SetData(ListingParams<M> listingDataModel, bool forIndex)
		{
			//read data from session only if the page is loaded for the first time (not AJAX search) 
			//but only if the page is not a lookup modal (these don't need to retain search parameters)
			//for AJAX search, all parameters are sent by the client
			var searchParams = (forIndex && !this.IsLookup) ? (Session[searchParamsSessionKey] as SearchParams) : null;

			if (searchParams == null)
				searchParams = new SearchParams();

			var pages = this.IsSubDetailPage ? new string[] { "5", "15", "50" } : new string[] { "15", "25", "50", "100" };
			var minPageSize = this.ListingViewModel.MinPageSize ?? (this.IsLookup ? 10 : (this.IsSubDetailPage ? 5 : 15));

			//if no advanced filters, try to retrieve from value providers
			if (listingDataModel.AdvancedFilters == null || listingDataModel.AdvancedFilters.Count == 0)
				listingDataModel.AdvancedFilters = DataExtensions.JSONToDictionary(this.ValueProvider.GetValue("advancedFilters"));

			//if no advanced filters, try to retrieve from session
			if (listingDataModel.AdvancedFilters == null || listingDataModel.AdvancedFilters.Count == 0)
				listingDataModel.AdvancedFilters = DataExtensions.JSONToDictionary(searchParams.AdvancedFilters);

			//current page index
			listingDataModel.PageIndex = (listingDataModel.PageIndex.HasValue) ? listingDataModel.PageIndex.Value - 1 : Convert.ToInt32(searchParams.PageIndex ?? 0);

			//number of rows per page
			if (listingDataModel.ItemsPerPage == 0)
			{
				//on lookup pages, the items per page is constant
				if (this.IsLookup)
					listingDataModel.ItemsPerPage = minPageSize;
				else
					listingDataModel.ItemsPerPage = Convert.ToInt32(searchParams.ItemsPerPage ?? minPageSize);
			}

			//keyword search
			if (listingDataModel.Keyword == null)
				listingDataModel.Keyword = "";

			if (String.IsNullOrEmpty(listingDataModel.Keyword))
				listingDataModel.Keyword = (searchParams.Keyword ?? "").ToString();

			//sorting; if not set from request, retrieve data from session
			if (listingDataModel.Sort == null)
			{
				listingDataModel.Sort = new ListingSortModel
				{
					Column = (searchParams.SortByColumn ?? "").ToString(),
					Asc = Convert.ToBoolean(searchParams.SortAsc ?? true)
				};
			}

			if (listingDataModel.Model == null)
				listingDataModel.Model = new M();

			//save data into session
			if (!this.IsLookup && !this.IsSubDetailPage)
			{
				searchParams.ItemsPerPage = listingDataModel.ItemsPerPage;
				searchParams.PageIndex = listingDataModel.PageIndex;
				searchParams.Keyword = listingDataModel.Keyword;
				searchParams.SortByColumn = listingDataModel.Sort.Column;
				searchParams.SortAsc = listingDataModel.Sort.Asc;
				searchParams.AdvancedFilters = listingDataModel.AdvancedFilters.ToJSON();

				//disable session
				//Session[this.searchParamsSessionKey] = searchParams;
			}

			//set view data
			ViewBag.ItemsPerPage = listingDataModel.ItemsPerPage;

			ViewBag.Pager = pages.ToSelectList(listingDataModel.ItemsPerPage.ToString(), false);
			ViewBag.Sorted = listingDataModel.Sort.Column;
			ViewBag.SortedAsc = listingDataModel.Sort.Asc;
		}

		protected PagedList<IModel> GetData(ListingParams<M> listingDataModel, bool forIndex = false)
		{
			this.SetData(listingDataModel, forIndex);

			return new PagedList<IModel>(repo.GetListingItems(listingDataModel), listingDataModel.PageIndex ?? -1, listingDataModel.ItemsPerPage);
		}

		public PagedList<IModel> GetLookupData(ListingParams<M> listingDataModel, bool forIndex = false)
		{
			this.SetData(listingDataModel, forIndex);
			ViewBag.Type = this.Name;

			return new PagedList<IModel>(repo.GetLookupItems(listingDataModel), listingDataModel.PageIndex ?? -1, listingDataModel.ItemsPerPage);
		}

		#endregion

		#region "Override"

		/// <summary>
		/// Function to be called after the item has been saved (edited or newly added)
		/// </summary>
		/// <param name="actionType"></param>
		/// <param name="model"></param>
		protected virtual Dictionary<string, object> AfterSave(DBActionType actionType, M model)
		{
			return null;
		}

		/// <summary>
		/// Returns meta parameters of the current model (e.g: its master id, its module, etc.)
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		protected virtual Dictionary<string, object> GetMetaParameters(M model)
		{
			return null;
		}

		/// <summary>
		/// Sets global data for all page types (List, Detail, Add)
		/// </summary>
		protected virtual void SetControllerData()
		{
			ViewBag.Menu = this.ParentMenu;
			ViewBag.SubMenu = (this.ListingViewModel.IsSubPage && this.SubMenu != Module.None) ? this.SubMenu.ToString() : this.Name;
			ViewBag.PageType = this.PageType;
			ViewBag.ItemTemplateName = this.ListingViewModel.ItemTemplateName;
		}

		/// <summary>
		/// Sets default search parameters when loading the listing page for the first time
		/// </summary>
		protected virtual void SetDefaultListingData(M model)
		{
		}

		/// <summary>
		/// Sets data for listing index page
		/// </summary>
		/// <param name="model"></param>
		protected virtual void SetIndexData(M model)
		{
			this.ListingViewModel.Parameters = this.GetMetaParameters(model);

			//set sub-type of model based on the bound values
			model.SetMetaData();
		}

		/// <summary>
		/// Sets data for lookup index page
		/// </summary>
		/// <param name="model"></param>
		protected virtual void SetLookupIndexData(M model)
		{
			this.ListingViewModel.Parameters = this.GetMetaParameters(model);
		}

		/// <summary>
		/// Sets data for listing page (including when requesting index page and when reloading listing through AJAX)
		/// </summary>
		/// <param name="model"></param>
		protected virtual void SetListingData(M model)
		{
			ViewBag.Name = this.Name;
			ViewBag.HasDetailPage = this.ListingViewModel.HasDetailPage;
			ViewBag.AllowDelete = this.ListingViewModel.AllowDelete;

			if (model == null)
				model = new M();

			ViewBag.ParentModule = model.ParentModule;

			//listing headers
			//if the page is lookup, the headers should be retrieved from the lookup item model
			if (this.IsLookup && this.ListingViewModel.Items != null && this.ListingViewModel.Items.Count() > 0)
				ViewBag.Headers = this.ListingViewModel.Items[0].GetLookupHeaders();
			//otherwise, the headers should be retrieved from the bound model
			else
				ViewBag.Headers = model.GetHeaders();

			ViewBag.IsLookup = this.IsLookup;
		}

		/// <summary>
		/// Sets data for detail pages (of type detail or add)
		/// </summary>
		/// <param name="model"></param>
		/// <param name="receivedModel"></param>
		protected virtual void SetDetailData(M model, M receivedModel = null)
		{
			this.SetControllerData();

			ViewBag.IsDetailMode = (this.PageMode == PageMode.Detail);
			ViewBag.IsAddMode = (this.PageMode == PageMode.Add);
			ViewBag.PageMode = this.PageMode;

			this.DetailViewModel = new DetailViewModel
			{
				Label = System.Text.RegularExpressions.Regex.Replace(this.name, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim(),
				Module = this.module,
				PageType = this.PageType,
				SelectedTab = (TempData[TempKey.SelectedTab.ToString()] ?? "").ToString(),
				Action = TempData[TempKey.DetailAction.ToString()].ToEnum<DetailAction>()
			};

			ViewBag.DetailViewModel = this.DetailViewModel;
		}

		#endregion

		#region "Actions"

		/// <summary>
		/// Sets general data for listing pages
		/// </summary>
		/// <param name="pageType"></param>
		/// <param name="isLookup"></param>
		/// <param name="listingDataModel"></param>
		private void SetListingParameters(string pageType, bool? isLookup, ListingParams<M> listingDataModel)
		{
			this.IsLookup = isLookup ?? false;
			this.PageType = pageType;
			this.SetControllerData();

			if (listingDataModel.Model != null)
				listingDataModel.Model.SetMetaData();
		}

		/// <summary>
		/// Returns the IDs of the items in the current listing
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public ActionResult GetDataIDs(string pageType, bool? isLookup, bool searchCriteriaChanged, ListingParams<M> listingDataModel, List<int> excludedIDs)
		{
			this.SetListingParameters(pageType, isLookup, listingDataModel);
			this.SetData(listingDataModel, false);

			IEnumerable<int> ids = repo.GetLookupItems(listingDataModel).Select(e => e.ID).ToList();

			if (excludedIDs != null && excludedIDs.Count > 0)
				ids = ids.Except(excludedIDs);

			return Json(ids);
		}

		/// <summary>
		/// Returns listing data determined by the seach parameters received from the clinet; called through AJAX
		/// </summary>
		/// <param name="pageType"></param>
		/// <param name="isLookup"></param>
		/// <param name="searchCriteriaChanged"></param>
		/// <param name="listingDataModel"></param>
		/// <returns></returns>

		// /POST/GetListing
		[HttpPost]
		public ActionResult GetListing(string pageType, bool? isLookup, bool searchCriteriaChanged, ListingParams<M> listingDataModel)
		{
			this.SetListingParameters(pageType, isLookup, listingDataModel);

			this.SetListingData(listingDataModel.Model);
			this.ListingViewModel.Items = this.IsLookup ? this.GetLookupData(listingDataModel) : this.GetData(listingDataModel);

			return View(GetViewName(ViewType.List), this.ListingViewModel.Items);
		}

		public ActionResult Index(int? id, string pageType, bool? isLookup, M model = null, ListingParams<M> listingParams = null)
		{
			this.PageType = pageType;
			this.IsLookup = isLookup ?? false;

			//if subpage, reset id
			if (pageType != null)
				id = null;

			var listingDataModel = new ListingParams<M>
			{
				PageIndex = id,
				ItemsPerPage = 0,
				Model = (M)model,
				Origin = (listingParams == null) ? null : listingParams.Origin
			};

			//set params
			this.ListingViewModel.PageIndex = id;
			this.ListingViewModel.ItemsPerPage = 0;

			this.SetDefaultListingData(model);
			this.CopyViewModelToDataModel(listingDataModel);
			this.ListingViewModel.Items = this.IsLookup ? this.GetLookupData(listingDataModel, true) : this.GetData(listingDataModel, true);
			this.ListingViewModel.Keyword = listingDataModel.Keyword;
			this.CopyDataModelToViewModel(listingDataModel);

			model.SetMetaData();
			if (this.IsLookup)
				this.SetLookupIndexData((M)model);
			else
				this.SetIndexData((M)model);
			model.SetMetaData();

			//make sure these are at the end
			this.SetListingData((M)model);
			this.SetControllerData();

			//if there is at least one advanced filter, show container by default
			if (this.ListingViewModel.AdvancedFilterValues != null && this.ListingViewModel.AdvancedFilterValues.Keys.Count > 0)
				this.ListingViewModel.IsAdvancedFilterOn = true;

			return View(GetViewName(ViewType.Index), this.ListingViewModel);
		}

		// GET/Detail
		[HttpGet]
		[ActionName("Detail")]
		public virtual ActionResult DetailGet(string id, M receivedModel = null, bool ignoreErrors = true)
		{
			//if received model cannot be bound, ignore error
			if (ignoreErrors)
				ModelState.Clear();

			if (String.IsNullOrEmpty(id))
				return RedirectToAction("Index", new { PageType = this.PageType });

			M model = repo.GetItem(id, receivedModel);

			//if record doesn't exist, go to index page
			if (model == null)
				return RedirectToAction("Index");

			this.DetailModel = model;
			this.PageMode = PageMode.Detail;

			model.SetMetaData();
			this.SetDetailData(model, receivedModel);
			model.SetMetaData();

			return View(GetViewName(ViewType.Detail), model);
		}

		// POST/Detail
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Detail")]
		public virtual ActionResult Detail(M model)
		{
			return this.Detail(model, null);
		}

		public ActionResult Detail(M model, Func<ActionStatus> saveFunction = null)
		{
			//set sub-type of model based on the bound values
			model.SetMetaData();

			if (!ModelState.IsValid)
			{
				TempData[Config.ActionStatus] = "Validation error";
				return DetailGet(model.ID.ToString(), null, false);
			}

			ActionStatus result = (saveFunction != null) ? saveFunction() : repo.SaveItem(model, DBActionType.Update);

			if (!result.Success)
			{
				TempData[Config.ActionStatus] = result.Message;
				return DetailGet(model.ID.ToString(), model);
			}
			else
			{
				TempData[Config.ActionStatus] = "Save successful";
				this.AfterSave(DBActionType.Update, model);
			}

			if (model.ParentID != null)
			{
				var parent = (model.ParentModule == Data.Models.Module.None) ? model.ParentModuleName : model.ParentModule.ToString();
				this.SetSelectedDetailTab(this.name, DetailAction.None);
				return RedirectToAction("Detail", parent, new { id = model.ParentID });
			}
			else
				return RedirectToAction("Detail", model.GetRouteParameters(model.ParentModule));
		}

		// POST/Add
		[HttpGet]
		[ActionName("Add")]
		public virtual ActionResult AddGet(M model = null, bool ignoreErrors = true)
		{
			if (ignoreErrors)
				ModelState.Clear();

			if (model == null)
				model = new M();

			this.PageMode = PageMode.Add;

			model.SetMetaData();
			this.SetDetailData(model, null);
			model.SetMetaData();
				
			return View(GetViewName(ViewType.Detail), model);
		}

		[HttpPost]
		public virtual ActionResult Add(M model = null)
		{
			return this.Add(model, null);
		}

		public ActionResult Add(M model, Func<ActionStatus> func)
		{
			if (!ModelState.IsValid)
				return AddGet(model, false);

			//set sub-type of model based on the bound values
			model.SetMetaData();

			ActionStatus result = (func != null) ? func() : repo.SaveItem(model, DBActionType.Insert);

			if (Request.IsAjaxRequest())
			{
				Dictionary<string, object> response = this.AfterSave(DBActionType.Insert, model);

				if (response == null)
					return Json(result.Success);
				else
				{
					response["Success"] = result.Success;
					return Json(response);
				}
			}

			if (!result.Success)
			{
				TempData[Config.ActionStatus] = String.IsNullOrEmpty(result.Message) ? "Insert error" : result.Message;
				return AddGet(model);
			}
			else
			{
				TempData[Config.ActionStatus] = "Save successful";
				this.AfterSave(DBActionType.Insert, model);

				if (model.ParentID != null)
				{
					var parent = (model.ParentModule == Module.None) ? model.ParentModuleName : model.ParentModule.ToString();
					this.SetSelectedDetailTab(this.name, DetailAction.Add);
					return RedirectToAction("Detail", parent, new { id = model.ParentID });
				}
				else
				{
					var action = (this.DetailViewModel == null) ? null : this.DetailViewModel.Actions[DetailAction.Add];

					if (action != null)
						return RedirectToAction(action.Action, action.Controller, action.Data);
					else
						return RedirectToAction("Detail", model.GetRouteParameters(model.ParentModule));
				}
			}
		}

		// POST/Delete
		[HttpPost]
		[ValidateAntiForgeryFormAndJSON]
		public virtual ActionResult Delete(M model)
		{
			//set sub-type of model based on the bound values
			model.SetMetaData();

			ActionStatus result = repo.DeleteItem(model);

			//if AJAX request, only return action status
			if (Request.IsAjaxRequest())
				return Json(result);
			//set action status message and redirect to appropriate page
			else
			{
				if (!result.Success)
				{
					TempData[Config.ActionStatus] = result.Message;
					return RedirectToAction("Detail", model.GetRouteParameters(model.ParentModule));
				}
				else
				{
					TempData[Config.ActionStatus] = "Delete successful";

					//if the delete was made on a sub-detail page, redirect to the parent detail page 
					//with the previous tab selected by default
					if (model.ParentID != null)
					{
						var parent = (model.ParentModule == Module.None) ? model.ParentModuleName : model.ParentModule.ToString();
						this.SetSelectedDetailTab(this.name, DetailAction.Delete);
						return RedirectToAction("Detail", parent, new { id = model.ParentID });
					}
					else
					{
						var action = (this.DetailViewModel == null) ? null : this.DetailViewModel.Actions[DetailAction.Delete];

						//if after delete action is provided, call that
						if (action != null)
							return RedirectToAction(action.Action, action.Controller, action.Data);
						//otherwise go to the index page
						else
							return RedirectToAction("Index");
					}
				}
			}
		}

		// Detail -> Add -> Cancel
		[HttpGet]
		public ActionResult Close(M model)
		{
			//set sub-type of model based on the bound values
			model.SetMetaData();

			if (model.ParentID != null)
			{
				var parent = (model.ParentModule == Module.None) ? model.ParentModuleName : model.ParentModule.ToString();
				this.SetSelectedDetailTab(this.name, DetailAction.Close);
				return RedirectToAction("Detail", parent, new { id = model.ParentID });
			}
			else
				return RedirectToAction("Index", this.name);
		}

		#endregion
	}
}
