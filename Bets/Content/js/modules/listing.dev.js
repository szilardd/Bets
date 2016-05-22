/// <reference path="../jquery/jquery-1.6.4.min.js"/>

var ListingPages = {};

///<summary>Provides functionality for listing pages (pagination, searching, sorting, etc.)</summary>
var ListingPage = function (parameters) {

	//flag which determines if the current listing module is used to show data inside a lookup page
	this.isLookup = parameters.isLookup;

	//if the listing is of type 'lookup', it holds the lookup page module
	this.lookup = parameters.lookup;

	//type of listing (e.g.: run, customer)
	this.listingType = parameters.listingType;

	//from which page does the lookup originate
	this.origin = parameters.origin;

	//type of page (more exactly sub-type); e.g: customerun, salescall-followup
	this.pageType = null;

	//contains parameters to be sent to the server
	this.$frmSearchParams = null;

	this.functions = {
		dataRetrievedCallback: parameters.dataRetrievedCallback //to be called after data has been loaded
	};

	var $page,
		$itemContainer,
		$advancedSearchForm,
		$table,
		$txtSearch,
		//object which contains all the search parameters (including advanced filters); this is sent to the server to retrieve the filtered result set
		searchParams = { pageIndex: 0, itemsPerPage: 0, keyword: '', advancedFilters: {}, sort: {} },
		DateFilter = { AllDates: 1, Custom: 2, ThisWeek: 3, LastWeek: 4, ThisMonth: 5, Today : 6, Yesterday : 7 },
		SearchMinLength = 3,
		that,

	///<summary>Binds pagination evetns (when clicking on the prev and next buttons in the listing)</summary>
	bindPagination = function () {

		//paginate
		$page.delegate(".pagination", "click", function (e) {
			e.preventDefault();
			searchParams.pageIndex = $(this).data("page-index");
			that.getContent();
		});

		//number of rows per page
		$page.delegate('#pager', 'change', function () {
			searchParams.itemsPerPage = $(this).val();
			that.getContent();
		});
	},

	///<summary>Binds delete event when clicking on the delete button in the listing</summary>
	bindDelete = function () {

		var $listingItem;

		$page.delegate('.btn-delete', 'click', function () {

			$listingItem = that.getListingItem($(this));

			Utils.confirm({
				message: "Are you sure you want to delete?",
				callback: function (status) {

					if (!status)
						return;

					AjaxUtils.post({
						blockElement	:	$itemContainer,
						json			:	true,
						url				:	that.listingType + "/Delete",
						dataType		:	"json",
						sendToken		:	true,
						data			:	JSON.stringify($.extend(Utils.getAntiForgeryToken(), that.$frmSearchParams.serializeObject(), {
												id			:	$listingItem.data("id"),
												pageType	:	that.pageType,
												timeStamp	:	$listingItem.data("timestamp")
											})),
						success			:	function () {
												that.getContent();
											}
					});
				}
			});
		});
	},

	///<summary>Binds search functionality (triggered when the user enters something in the search box)</summary>
	bindSearch = function () {

		var keyupTimeoutID,
			keyword;

		searchParams = {
			pageIndex: $page.find('.current-page').val(),
			itemsPerPage: $page.find('.pager').val(),
			keyword: $txtSearch.val(),
			advancedFilters: {},
			sort: {},
			searchCriteriaChanged: false
		};

		//bind keyword search
		$txtSearch.bind('keyup', function (e) {

			if (e.which === 13) {
				that.getContent();
				return;
			}

			//clear search timeout
			if (keyupTimeoutID)
				clearTimeout(keyupTimeoutID);

			//don't call search immediately; wait for 500 milliseconds
			keyupTimeoutID = setTimeout(function () {

				keyword = $.trim($txtSearch.val());

				//search when filter length is at least 4 characters, or the search is called explicitly, or backspace was pressed 
				if (keyword.length >= SearchMinLength || keyword === '') { //backspace

					searchParams.pageIndex = 1;
					searchParams.keyword = keyword;
					searchParams.searchCriteriaChanged = true;

					that.getContent();
				}

			}, 500);
		});

		//bind clearing keyword search
		$page.find('.btn-cancel-search').click(function () {

			if ($txtSearch.val() != '') {

				$txtSearch.val('');

				searchParams.keyword = ''; //reset keyword search
				searchParams.pageIndex = 1; //reset page
				searchParams.searchCriteriaChanged = true;
				searchParams.sort = {}; //reset sort column ?
				that.getContent();
			}
		});
	},

	///<summary>Binds advanced filters (shown when clicking on the 'Advanced Search' button)</summary>
	bindAdvancedSearch = function () {

		if ($advancedSearchForm.length == 0)
			return;

		$advancedSearchForm.find(':input').each(function () {

			var $this = $(this),
				value = $(this).val();

			if ($this.is('select')) {
				if (value != Config.AllItems)
					searchParams.advancedFilters[this.id] = value;
			}
			else if ((this.id == "FromDate" || this.id == "ToDate") && value != "")
				searchParams.advancedFilters[this.id] = Utils.dateToISO(Utils.convertISODateToObject(value));
			else if (value != "")
				searchParams.advancedFilters[this.id] = value;
		});

		//bind actions container toggling (show/hide containe when tapping the "Advanced" link) 
		var $advancedSearchClick = $page.find('.advanced-filter:first');
		$advancedSearchClick.click(function () {

			$page.find('.advanced-search:first').slideToggle('medium');

			var status = $(this).css('background-image').indexOf("arrow-up") >= 0 ? "arrow-up" : "arrow-down",
				reverseStatus = (status == "arrow-up") ? "arrow-down" : "arrow-up";

			//change arrow orientation based on the status of the container
			$(this)
				.toggleClass("active")
				.css('background-image', $(this).css('background-image').replace(status, reverseStatus));
		});

		//check if the advance filter defaultly is on
		if ($advancedSearchClick.hasClass('advanced-filter-on'))
			$advancedSearchClick.trigger('click');

		bindAdvancedDropdownFilters();
		bindAdvancedDateFilters();
	},

	///<summary>Binds change event for dropdown advanced filters</summary>
	bindAdvancedDropdownFilters = function () {

		//dropdown filters
		$advancedSearchForm.find('select').change(function () {

			var $this = $(this),
				type = $this.attr("name"),
				value = $this.val();

			setAdvancedFilterSearchValue({
				$element: $this,
				fieldName: type,
				value: value
			});

			//if there is another filter which depends on this one, do not trigger content retrieval
			//because it will be triggered by the dependent filter
			if ($this.data("dependency"))
				return;

			searchParams.searchCriteriaChanged = true;
			searchParams.pageIndex = 1;
			that.getContent();
		});
	},

	///<summary>
	/// Sets the value of the selected advanced search item into the search filter
	///</summary>
	setAdvancedFilterSearchValue = function (parameters) {

		var type = parameters.fieldName,
			value = parameters.value,
			$this = parameters.$element,
			isDateFilter = $this.attr("name") == "DateFilter";

		//if the selected option is the default option, remove it from filters
		if (value === null || value === $this.children(':first').attr("value")) {

			delete searchParams.advancedFilters[type];

			if (isDateFilter) {

				$advancedSearchForm.find('.date-field').val("");
				delete searchParams.advancedFilters["FromDate"];
				delete searchParams.advancedFilters["ToDate"];
			}
		}
		//add filter to advanced filters
		else {
			searchParams.advancedFilters[type] = value;

			//if the type of the field is date
			if (isDateFilter) {

				var $fromDate = $advancedSearchForm.find('#FromDate'),
					$toDate = $advancedSearchForm.find('#ToDate'),
					monday = Date.today().previous().monday(),
					start,
					end;

				value = parseInt(value, 10);

				switch (value) {

					case DateFilter.LastWeek: { start = new Date(monday).add(-7).days(); end = new Date(monday).add(-1).days(); break; }
					case DateFilter.ThisWeek: { start = new Date(monday); end = new Date(monday).add(6).days(); break; }
					case DateFilter.ThisMonth: { start = Date.today().clearTime().moveToFirstDayOfMonth(); end = Date.today().clearTime().moveToLastDayOfMonth(); break; }
					case DateFilter.Today: { start = Date.today(); end = Date.today().addDays(1).addSeconds(-1); break; }
					case DateFilter.Yesterday: { start = Date.today().addDays(-1); end = Date.today().addSeconds(-1); break; }
					default: { start = null; end = null; }
				}

				//only add date filter to the advanced filters if start and end date are selected
				if (start != null) {

					$fromDate.datepicker('setDate', start);
					$toDate.datepicker('setDate', end);

					searchParams.advancedFilters['FromDate'] = Utils.dateToISO(start);
					searchParams.advancedFilters['ToDate'] = Utils.dateToISO(end);
				}
			}
		}
	},

	///<summary>Binds events for date field advanced filters</summary>
	bindAdvancedDateFilters = function () {

		//date inputs
		$advancedSearchForm.find('.date-field').each(function () {

			var $this = $(this);

			//bind the date picker
			Utils.addDateField(this);

			$this.change(function () {

				var $fromDate = $advancedSearchForm.find('#FromDate'),
					$toDate = $advancedSearchForm.find('#ToDate'),
					fromDate = $fromDate.val(),
					toDate = $toDate.val();

				//only trigger search if from and to values are given
				if (fromDate != '' && toDate != '') {

					fromDate = $fromDate.datepicker('getDate');
					toDate = $toDate.datepicker('getDate');

					searchParams.advancedFilters['FromDate'] = Utils.dateToISO(fromDate);
					searchParams.advancedFilters['ToDate'] = Utils.dateToISO(toDate);

					$('[name=DateFilter]').val(DateFilter.Custom);
					searchParams.advancedFilters['DateFilter'] = DateFilter.Custom;

					searchParams.searchCriteriaChanged = true;
					searchParams.pageIndex = 1;
					that.getContent();
				}
			});
		});
	},

	///<summary>Saves custom search parameters</summary>
	bindSavedSearch = function () {

		//save search
		$page.find('.btn-save-search:first').click(function () {

			Utils.confirm({
				message		:	"Save this Search:",
				hasInput	:	true,
				callback	:	function (status, text) {

									if (!status)
										return;

									AjaxUtils.post({
										blockElement	:	$advancedSearchForm,
										json			:	true,
										url				:	"User/AddSavedSearch",
										dataType		:	"json",
										data			:	JSON.stringify({
																name			:	text,
																pageType		:	that.pageType,
																keyword			:	searchParams.keyword,
																advancedFilters	:	JSON.stringify(searchParams.advancedFilters)
															}),
										success			:	function () {}
									});
								}
			});
		});

		//binds event for showing saved search modal when clicking on the 'My Saved Searches' on the listing page
		$page.find('.btn-show-searches:first').click(function () {
		
			AjaxUtils.get({
				url		:	"User/GetSavedSearches",
				data	:	{ pageType: that.pageType },
				success	:	function (content) {

								Utils.modal({
									content		: content,
									onClosed	: function () { },
									onComplete	: bindSavedSearchActions
								});
							}
			});
		});


		//cancel advanced filters
		$page.find('.btn-clear-filters').click(function() {
		
			//reset advanced filters
			searchParams.advancedFilters = {};
			
			//clear selected values
			var $advancedFilters = $advancedSearchForm.find(":input"),
				$dependentFilters = $advancedFilters.filter("[data-dependency]");

			//if there are no dependent filters, reset selected values and reload listing
			if ($dependentFilters.length === 0) {

				$advancedFilters.val(Config.AllItems);
				
				//reload listing
				that.getContent();
			}
			//otherwise trigger the reload by resetting first dependend filter
			else
				$($dependentFilters[0]).val(Config.AllItems).change();
		});
	},

	///<summary>Binds event in the save search modal (selecting and deleting saved search)</summary>
	bindSavedSearchActions = function () {

		var $container = $("#modal-saved-searches ul");

		$container.delegate("li", "click", function (e) {

			var $this = $(e.target),
				$li = $this.is("li") ? $this : $this.parents("li:first"),
				id = $li.data("id");

			//delete search
			if ($this.hasClass("btn-delete")) {

				AjaxUtils.post({
					blockElement	:	$container,
					url				:	"User/DeleteSavedSearch",
					data			:	{ id: id },
					success			:	function () {
											$li.remove();
										}
				});
			}
			//set search as active
			else {

				AjaxUtils.post({
					blockElement	:	$container,
					url				:	"User/SelectSavedSearch",
					data			:	{ id: id, pageType: that.pageType },
					dataType		:	"json",
					success			:	function (data) {

											var $rootDependentDropdown;

											data.AdvancedFilters = JSON.parse(data.AdvancedFilters);

											//reset form
											$advancedSearchForm.find(":input").val(Config.AllItems);
											$('[data-dependenton]').attr('disabled','disabled');

											searchParams.advancedFilters = {};

											//set advanced search filter values
											for (var name in data.AdvancedFilters) {

												var $field = $advancedSearchForm.find('[name=' + name + ']');

												//if the field are date fields, set the date using jquery UI datepicker API
												if (name == "FromDate" || name == "ToDate")
													$field.datepicker('setDate', Utils.convertISODateToObject(data.AdvancedFilters[name]));
												else {
													
													//if the field is the root dependent dropdown
													if ($field.data("dependency") && !$field.data("dependenton")) {
														$rootDependentDropdown = $field;
													}
													//otherwise if the field is a child dependent dropdown
													else if ($field.data("dependenton")) {

														//bind reloaded event to be triggered when the dropdown is populated
														//so that the search value can be set
														if (!$field.hasEvent('reloaded')) {

															$field.bind('reloaded', function () {

																var $this = $(this);

																//filter contains values, so it must be enabled
																$this.removeAttr('disabled');
															
																//set value from saved search and trigger change to cascade reloading
																$this.val(data.AdvancedFilters[$this.attr('name')]).change();
															});
														}
													}
													//otherwise the field is a regular advanced filter
													else
														$field.val(data.AdvancedFilters[name]);
												}

												//store values in search params so it will be passed to the server when reloading data
												searchParams.advancedFilters[name] = data.AdvancedFilters[name];
											}

											//set keyword
											searchParams.keyword = data.Keyword;
											$txtSearch.val(data.Keyword);

											//if the saved search contains values for dependent drodpwons, trigger the change for the root filter;
											//this will trigger the changes in cascade and reload the listing
											if ($rootDependentDropdown)
												$rootDependentDropdown.val(data.AdvancedFilters[$rootDependentDropdown.attr('name')]).change();
											//otherwise retrieve data
											else
												that.getContent();

											Utils.closeModal();
										}
				});
			}
		});
	},

	///<summary>Binds sort functionality (when clicking on a table column header in the listing page)</summary>
	bindSort = function () {

		//no sorting on lookup pages
		if (that.isLookup)
			return;

		var $columns = $table.find('th');

		$columns.click(function () {

			var $this = $(this),
				//name of the field is stored in data-col
				column = $this.data('col'),
				//ascending status is stored in data-desc
				ascending = !$this.hasClass('desc');

			if (!column)
				return;

			//always change sort order by negating the current order
			searchParams.sort = { 'Column': column, 'Asc': !ascending };
			that.getContent();

			$columns.removeClass('sorted');
			$this.addClass('sorted');
		});
	},

	///<summary>Binds event for selecting all listing rows when clicking on the 'Select all' checkbox in the listing table</summary>
	bindSelectAll = function () {

		$itemContainer.find(".select-all").change(function () {
			var status = $(this).attr("checked") || false;

			$table.find(".action > input").attr("checked", status);
		});
	},

	///<summary>Binds events for showing lookup pages</summary>
	bindLookup = function () {

		var $this;

		//lookup pages are triggered for element that have data-lookup attribute
		$page.find('[data-lookup]').click(function () {

			$this = $(this);

			//the lookup works in 'single' select and also 'multiple' select mode;
			//'mulitple' mode can be set also to 'select all' mode
			var selectAll = $this.data('selectall'),
				masterData = {};
				
			masterData[$this.data('field')] = $this.data('id');

			//if not specified, default to true
			if (selectAll === undefined)
				selectAll = true;

			//bind lookup modal window
			var lookup = new LookupPage({
				parent			:	that,
				multipleAdd		:	true,
				selectAll		:	selectAll,
				listingType		:	$this.data('lookup'), //type of lookup is stored in data-lookup
				pageType		:	$this.data('pagetype'), //type of page is stored in data-pagetype
				searchParams	:	$.extend(that.$frmSearchParams.serializeObject(), masterData),
				onComplete		:	function() {
										EventManager.trigger(EventManager.Event.LookupReload, that.pageType, lookup);
									},
				addCallback		:	function (id, name) {
									}
			});
			
			lookup.show();
		});
	};

	///<summary>Retrieves listing data from server based on the search parameters</summary>
	this.getData = function(parameters) {

		AjaxUtils.post({
			json			:	true,
			url				:	that.listingType + "/" + parameters.type,
			data			:	JSON.stringify($.extend(parameters.params, {
									model					:	that.$frmSearchParams.serializeObject(),
									pageType				:	that.pageType,
									pageIndex				:	searchParams.pageIndex,
									itemsPerPage			:	searchParams.itemsPerPage,
									keyword					:	searchParams.keyword,
									searchCriteriaChanged	:	searchParams.searchCriteriaChanged,
									advancedFilters			:	JSON.stringify(searchParams.advancedFilters),
									sort					:	searchParams.sort,
									isLookup				:	that.isLookup,
									origin					:	that.origin
								})),
			dataType		:	parameters.json ? "json" : "text",
			blockElement	:	$itemContainer,
			success			:	function (content) {
									parameters.callback(content);
								}
		});
	}

	///<summary>Reloads listing data based on the search parameters</summary>
	this.getContent = function () {

		that.getData({
			type		:	"GetListing", 
			callback	:	function(content) {

								searchParams.searchCriteriaChanged = false;

								$itemContainer.html(content);

								$table = $page.find('.listing-table:first');

								//rebind events
								bindSort();
								bindSelectAll();

								//set page index
								searchParams.pageIndex = $page.find('.current-page').val();

								//call function if defined
								if (that.functions.dataRetrievedCallback)
									that.functions.dataRetrievedCallback();

								//trigger reload events
								if (that.lookup)
									EventManager.trigger(EventManager.Event.LookupReload, that.pageType, that.lookup);
								else
									EventManager.trigger(EventManager.Event.ListingReload, that.pageType, that.lookup);
							}
		});
	};

	this.init = function () {

		that = this;

		$page = that.isLookup ? $('#lookup-' + that.listingType) : $("[data-listingtype=" + that.listingType + "]:first");
		that.pageType = $page.data("pagetype");

		//cache listing object
		ListingPages[that.listingType] = this;

		that.$frmSearchParams = $page.find(".frm-search:first");
		$table = $page.find('.listing-table:first');
		$itemContainer = $page.find(".listing-items:first");
		$advancedSearchForm = $page.find(".advanced-search:first");
		$txtSearch = $page.find('.txt-listing-search');

		bindPagination();
		bindDelete();
		bindSearch();
		bindAdvancedSearch();
		CRUD.bindDependentDropdowns($advancedSearchForm, that.listingType, CRUD.PageMode.List);
		bindSavedSearch();
		bindSort();
		bindSelectAll();

		if (!that.isLookup)
			bindLookup();
	};

	this.reload = function () {
		this.getContent();
	};

	///<summary>
	// Returns the parent listing item using a target element (which is the child of the listing item)
	///</summary>
	this.getListingItem = function($target) {

		if (that.isLookup) {

			if ($target.hasClass("listing-item"))
				return $target;

			return $target.parents('.listing-item');
		}
		else {

			if ($target.data('id'))
				return $target;

			return $target.parents('.listing-item');
		}
	};

	///<summary>Returns listing item's ID</summary>
	this.getListingItemID = function ($target) {

		var $listingItem = that.getListingItem($target);
		return $listingItem.data('id');
	}

	///<summary>Retrieves only the ids of the listing items based on the current search parameters</summary>
	this.getListingItemIDs = function(excludedIDs, callback) {

		this.getData({
			json		:	true,
			type		:	"GetDataIDs",
			callback	:	callback,
			params		:	{ excludedIDs : excludedIDs }
		});
	}

	this.moveToNextPage = function() {
		searchParams.pageIndex += 1;
		this.reload();
	};

	this.moveToPrevPage = function() {
		searchParams.pageIndex += 1;
		this.reload();
	};
};

$(function() { 

	$("[name=fld-listing-type]").each(function() {

		var listing = new ListingPage({ listingType : $(this).val() });
		listing.init();
	});
});