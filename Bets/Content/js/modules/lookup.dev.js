/// <reference path="../jquery/jquery.min.js"/>

///<summary>
/// Module which provides functionality for lookup modal pages: retrieving, searching, adding, selecting item(s)
///</summary>
var LookupPage = function (parameters) {

	var that = this;

	//parent object which created the lookup
	this.parent = parameters.parent;

	//type of lookup
	this.listingType = parameters.listingType;

	//sub page type
	this.pageType = parameters.pageType;

	//from which page does the lookup originate
	this.origin = parameters.origin;

	//HTML id of the page
	this.pageID = "lookup-" + parameters.listingType;

	//custom search params to be sent to the server when loading data
	this.searchParams = typeof (parameters.searchParams) === "function" ? parameters.searchParams() : parameters.searchParams;

	//page container
	this.$page = null;

	//listing object which handles the functionality for loading and searching data
	this.listing = null;

	this.functions = {
		addCallback	:	parameters.addCallback, //function to be called after item is selected
		onComplete	:	parameters.onComplete
	};

	//insert more than one in the database
	this.multipleAdd = parameters.multipleAdd || false;

	//if true, allow 'select all' mode
	this.selectAll = parameters.selectAll || false;

	//true if the data is added using a custom save function; otherwise default to ajax save
	this.customAdd = parameters.customAdd || false;

	//if true, it means that the ids in the selectedIDs array should be saved,
	//if false, it means that ONLY the ids in the selectedIDs array should NOT be saved
	this.selectAllMode = false;

	//if remove mode is true it means that there are items which can be removed
	this.removeMode = false;

	//an array containing the ids (if multipleAdd = true)
	this.selectedIDs = [];

	//array containing removed ids (only in 'removed' mode)
	this.removedIDs = [];

	this.selectedNames = [];
	this.nrOfSelectedIDs = 0;
	this.totalNrOfIDs = 0;

	this.init = function () {

		//empty previously selected items
		this.selectedIDs = [];
		this.removedIDs = [];
		this.selectedNames = [];

		if (parameters.data && parameters.data.ids) {
			this.selectedIDs = parameters.data.ids;
			this.selectedNames = parameters.data.names;
		}
	};

	///<summary>Retrieves modal content and displays as overlay</summary>
	this.show = function () {

		that.getModalContent(function (content) {
			that.displayModal(content);
		});
	};

	///<summary>Retrieves HTML content for the modal</summary>
	this.getModalContent = function (callback) {

		AjaxUtils.post({
			json			:	true,
			url				:	that.listingType,
			data			:	JSON.stringify({
									isLookup	:	true,
									pageType	:	that.pageType,
									model		:	that.searchParams,
									origin		:	that.origin
								}),
			blockElement	:	$("body"),
			success			:	function (content) {
									that.displayModal(content);
								}
		});
	};

	///<summary>Display modal using jquery.fancybox</summary>
	this.displayModal = function (content, callback) {

		//bind overlay trigger to the current element
		Utils.modal({
			content		:	content,
			onComplete	:	function () {

								if (callback)
									callback();
								else
									that.bindModal();

								//trigger callback to mark selected items
								that.dataRetrievedCallback();

								//notify listeners that the lookup has been shown
								if (that.functions.onComplete)
									that.functions.onComplete();
							},
			onClosed	:	function () { 
								EventManager.trigger(EventManager.Event.LookupClose, that.pageType);
							}
		});
	};

	///<summary>Binds events and initializes listing page</summary>
	this.bindModal = function () {

		that.$page = $('#' + that.pageID);

		if (that.selectAll)
			that.$page.addClass('lookup-multiple');

		that.bindUI();

		that.listing = new ListingPage({
			listingType				:	that.listingType,
			origin					:	that.origin,
			isLookup				:	true,
			dataRetrievedCallback	:	that.dataRetrievedCallback,
			lookup					:	this
		});

		that.listing.init();
	};

	this.close = function () {
		$.fancybox.close();
	};

	///<summary>Binds UI events and sets UI data (selecting items)</summary>
	this.bindUI = function () {

		var $this, $listingItem, $btnLookupAdd, itemID, index, name;

		//bind click handler for the add button and when clicking on row
		that.$page.delegate('.listing-item:not(.selected-item), .btn-lookup-add', 'click', function (e) {

			//make sure that only one delegate event is processed
			e.stopPropagation();
			
			$this = $(this);

			//determine listing item and lookup button
			if ($this.hasClass('listing-item')) {
				$btnLookupAdd = $this.find('.btn-lookup-add');
				$listingItem = $this;
			}
			else {

				$btnLookupAdd = $this;
				$listingItem = $this.parents('.listing-item:first')
			}

			//set item name
			name = $listingItem.find('.listing-item-name').text();

			//set item's id
			itemID = $listingItem.data('id');

			//if multiple add, store selected id, otherwise call callback
			if (that.multipleAdd) {

				//if 'select all' mode, remove the ID from array
				if (that.selectAllMode) {

					index = that.selectedIDs.indexOf(itemID);

					if (index >= 0) {

						that.selectedIDs.splice(index, 1);
						that.nrOfSelectedIDs += 1;
						that.setCount();
					}
				}
				else {

					//if 'removed' mode and the item has already been removed, make sure that
					//it won't be marked as removed anymore
					if (that.removeMode) {
					
						index = that.removedIDs.indexOf(itemID);

						//if item has already been removed, remove from array
						if (index >= 0)
							that.removedIDs.splice(index, 1);
						//otherwise mark item as selected
						else {
							that.selectedIDs.push(itemID);
							that.selectedNames.push(name);
						}

					}
					//if 'single select' mode, add id to array
					else {

						that.selectedIDs.push(itemID);
						that.selectedNames.push(name);
					}
				}

				//go to 'remove' state
				that.toggleButtonState.call($btnLookupAdd, false, itemID);
			}
			else {

				//if there is extra data assigned to the item, provide that also to the caller
				var $frmExtraData = $listingItem.find('.frm-extra-data'),
					extraData = {};

				if ($frmExtraData.length == 1)
					extraData = $frmExtraData.serializeObject();

				//call callback to notify caller that an item has been selected
				that.functions.addCallback(itemID, name, extraData);
				that.close();

				//trgger add event
				EventManager.trigger(EventManager.Event.LookupAdd, that.pageType, { itemID : itemID, name : name, extraData: extraData });
			}
		});

		//bind multiple add specific events
		if (that.multipleAdd)
			that.bindMultipleAdd();
	};

	///<summary>
	/// Binds events for lookup which works in 'multiple' mode
	///</summary>
	this.bindMultipleAdd = function() {

		var $this, $listingItem, $btnLookupRemove, itemID, index;

		//bind click handler for remove button
		that.$page.delegate('.selected-item, .btn-lookup-remove', 'click', function (e) {

			//make sure that only one delegate event is processed
			e.stopPropagation();

			var index;

			$this = $(this);

			//determine listing item and lookup button
			if ($this.hasClass('listing-item')) {
				$btnLookupRemove = $this.find('.btn-lookup-remove');
				$listingItem = $this;
			}
			else {

				$btnLookupRemove = $this;
				$listingItem = $this.parents('.listing-item:first')
			}

			//determine item's id
			itemID = $listingItem.data('id');

			//if 'select all' mode, add the ID to selected list
			if (that.selectAllMode) {
				that.selectedIDs.push(itemID);
				that.nrOfSelectedIDs -= 1;
				that.setCount();
			}
			//if 'single select' mode, remove id from array
			else {

				index = that.selectedIDs.indexOf(itemID);

				//if 'removed' mode and the item has already been selected, make sure that
				//it won't be marked as removed anymore
				if (that.removeMode) {
					
					//if item has not been already selected, mark as removed
					if (index < 0)
						that.removedIDs.push(itemID);
					//otherwise remove item from selected list
					else {
						that.selectedIDs.splice(index, 1);
						that.selectedNames.splice(index, 1);
					}

				}
				//otherwiser remove item from selected list
				else {

					that.selectedIDs.splice(index, 1);
					that.selectedNames.splice(index, 1);
				}
			}

			//revert to 'add' state
			that.toggleButtonState.call($btnLookupRemove, true, itemID);
		});

		//bind click handler for add all button (when multiple add mode)
		that.$page.find('.btn-add-all').removeClass('removed').click(function () {

			$this = $(this);

			//save only when there are selected items
			if (that.selectedIDs.length == 0 && that.removedIDs.length == 0 && !that.selectAllMode)
				return;

			$this.addClass('removed');

			//if 'select all' mode, retrieve all ids and call save after that
			if (that.selectAllMode) {

				that.listing.getListingItemIDs(that.selectedIDs, function(ids) {
					that.selectedIDs = ids;
					that.saveMultiple(that.selectedIDs);
				});
			}
			//if custom add, there is a custom save function; only pass ids and names
			else if (that.customAdd) {
				that.functions.addCallback(that.selectedIDs, that.selectedNames);
				that.close();
			}
			else {

				//save data received from custom add event
				var addCallback = function(data) {
					that.saveMultiple(data);
				};

				//if custom add event is defined, call that
				var triggered = EventManager.trigger(EventManager.Event.LookupAdd, that.pageType, that.selectedIDs, addCallback);
				
				//if no custom event, call default save
				if (!triggered)
					that.saveMultiple(that.selectedIDs);
			}
		});

		if (!that.customAdd)
			that.bindSelectAll();
	};

	///<summary>Binds functionality related to 'select all' mode</summary>
	this.bindSelectAll = function() {

		var $this, itemID, index;
		
		//bind click handler for the select all button
		that.$page.delegate('.btn-select-all', 'click', function () {

			var $this = $(this);

			//reset select mode
			that.selectAllMode = true;
			that.selectedIDs = [];

			//toggle button state
			that.toggleSelectAllButtonState.call($this);
			that.$page.find('.btn-lookup-add').click();

			//show count
			that.nrOfSelectedIDs = that.getTotalCount();
			that.totalNrOfIDs = that.nrOfSelectedIDs;
			that.setCount();
			that.$page.find('#listing-count-container').toggleClass('removed');
		});

		//bind click handler for the remove all button
		that.$page.delegate('.btn-remove-all', 'click', function () {
				
			var $this = $(this);

			//reset select mode
			that.selectAllMode = false;
			that.selectedIDs = [];

			//toggle button state
			that.toggleSelectAllButtonState.call($this);
			that.$page.find('.btn-lookup-remove').click();

			//hide count
			that.$page.find('#listing-count-container').toggleClass('removed');
		});
	};

	///<summary>
	/// Sends ajax request to save items when the lookup is in 'multiple' mode
	///</summary>
	this.saveMultiple = function(data) {

		var dataToSend = {
			ids		:	data,
			model	:	that.searchParams
		};

		//if removed mode, send removed ids also
		if (that.removeMode)
			dataToSend.removedIDs = that.removedIDs;

		AjaxUtils.post({
			json			:	true,
			dataType		:	"json",
			url				:	that.listingType + "/Add" + that.pageType,
			data			:	JSON.stringify(dataToSend),
			blockElement	:	$("body"),
			success			:	function (content) {

									if (that.parent)
										that.parent.reload();
									that.close();
								}
		});
	};

	///<summary>
	/// Called after listing has finished loading current page. 
	/// Marks records as selected stored in the id array
	///</summary>
	this.dataRetrievedCallback = function() {

		var $this, itemID, i,
			//all 'select' buttons from listing
			$buttons = that.$page.find(".listing-items tbody .button-action");

		//set state of 'select all' button
		if (that.selectAll) {

			var $btnSelectAll = that.$page.find('.btn-select-all');
			
			$btnSelectAll.removeClass('removed');

			if (that.selectAllMode)
				that.toggleSelectAllButtonState.call($btnSelectAll);
		}

		//if 'select all' mode, refresh selected count
		if (that.selectAllMode) {

			var totalCount = that.getTotalCount();

			if (totalCount != that.totalNrOfIDs) {
				that.nrOfSelectedIDs = totalCount;
				that.totalNrOfIDs = that.nrOfSelectedIDs;
				that.selectedIDs = [];
			}
			
			that.$page.find('#listing-count-container').removeClass('removed');
			that.setCount();
		}

		//if there are selected items, mark them as selected
		if (that.selectedIDs.length > 0) {

			$buttons.each(function () {

				$this = $(this);
				itemID = that.listing.getListingItemID($this);

				index = that.selectedIDs.indexOf(itemID);
				
				//if selected id is shown on the current page
				if (index >= 0) {

					//if 'select all' mode, deselect item
					if (that.selectAllMode)
						that.toggleButtonState.call($this, true, itemID);
					//if 'single select' mode, select item
					else
						that.toggleButtonState.call($this, false, itemID);
				}
				//otherwise, if 'select all' mode, select item
				else if (that.selectAllMode) {
					that.toggleButtonState.call($this, false, itemID);
				}				
			});
		}
		else if (that.selectAllMode) {

			$buttons.each(function() {

				var $this = $(this);
				itemID = that.listing.getListingItemID($this);

				that.toggleButtonState.call($(this), false, itemID);
			});
		}
	};

	///<summary>
	/// Toggles button state from removed to selected and from selected to removed
	///<summary>
	this.toggleButtonState = function(toRemovedState, itemID) {
		
		var $listingItem = this.parents('.listing-item:first');

		if (this.hasClass('btn-lookup-remove') || toRemovedState === true) {

			this.removeClass('btn-lookup-remove').addClass('btn-lookup-add').text('Select');
			$listingItem.removeClass('selected-item');
		}
		else if (this.hasClass('btn-lookup-add') || toRemovedState === false) {

			this.removeClass('btn-lookup-add').addClass('btn-lookup-remove').text('Remove');
			$listingItem.addClass('selected-item');
		}

		//trigger custom toggle
		EventManager.trigger(EventManager.Event.LookupItemStateChange, that.pageType, {
			toRemovedState	:	toRemovedState,
			itemID			:	itemID
		});
	};

	///<summary>
	/// Toggles 'all' button state from removed to selected and from selected to removed
	///</summary>
	this.toggleSelectAllButtonState = function() {
		
		if (this.hasClass('btn-select-all'))
			this.removeClass('btn-select-all').addClass('btn-remove-all').text('Remove All');
		else
			this.removeClass('btn-remove-all').addClass('btn-select-all').text('Select All');
	};

	///<summary>
	/// Removes item from the selected ones
	///</summary>
	this.removeItem = function(itemID) {

		var $button = that.$page.find("[data-id=" + itemID + "] .btn-lookup-remove");

		if ($button.length)
			that.toggleButtonState.call($button);
	};

	///<summary>
	/// Displays the number of selected records
	///</summary>
	this.setCount = function() {
		that.$page.find('#listing-count').text(that.nrOfSelectedIDs);
	};

	///<summary>
	/// Returns total number of items in the lookup listing
	///</summary>
	this.getTotalCount = function() {
		return parseInt($('#fld-total-count').val());
	};

	///<summary>
	/// Toggles removed mode
	///</summary>
	this.setRemoveMode = function(removeAllowed) {

		that.removeMode = removeAllowed;

		if (removeAllowed)
			that.$page.find('.btn-add-all').text("Save All");
	};

	this.init();
};