/// <reference path="../jquery/jquery-1.6.4.min.js"/>

///<summary>
/// Provides functionality for listing and detail pages
///</summary>
var CRUD = (function() {

	var PageMode = { Add : 1, Detail : 2, List : 3 };

	///<summary>
	/// Binds dependent dropdowns (dropdowns that depend on a parent value) inside a given container
	///</summary>
	function bindDependentDropdowns($container, pageType, pageMode) {

		$container.find("[data-dependenton]").each(function() {

			var $this = $(this),
				dependentOn = $this.data("dependenton");

			if (!dependentOn)
				return;
			
			//if the dropdown is inline dependent (detail fields), all the values are retrieved into the dropdown
			//and every <option> contains its parent value in data-parent
			if ($this.data("inlinedependent")) {

				//when the value of the dropdown is changed, determine parent value and set this value
				//in the parent dropdown and trigger its change event to propagate change on its parent (if necessary)
				$this.change(function() {

					var parentValue = $this.find(":selected").data("parent"),
						$parent = $('#' + dependentOn);

					//set focus (dirty tracking), set parent value and trigger change to propagate dependency
					$parent.dirtyval(parentValue);

					//set text of display field
					$parent.siblings('.display-field').text($parent.children(":selected").text());
				});

				//only the bottom level dropdown should be changeable, all others should change
				//their value dynamically; because of this, the parent dropdowns have to be marked as read-only
				$container.find('#' + dependentOn).each(function() {
					
					var $this = $(this);

					$this.addClass('removed');

					//create div which will act as the display field of the select
					var $div = $("<div>", {
						text	:	$this.children(":selected").text(),
						"class"	:	'display-field edit-field removed'
					}).data("visibility", "removed");

					//and insert it after select
					$this.after($div);
				});
			}
			//the dropdowns are block dependent (advanced filters); their parent values have to be retrived from the server;
			//bind functionality using jquery.dependent-dropdown
			else {

				$this.dependent({
					$container	:	$container,
					dependency	:	dependentOn,
					values		:	function($this, el, callback) { 
										
										var $el = $(el),
											value = $el.val();

										if (value == '' || $el.attr('disabled')) {
											callback([]);
											return;
										}

										//when the dropdown's value is changed, retrieve parent values from server
										//and populate child dropdown
										AjaxUtils.post({
											url		:	pageType + '/GetDependentDropdownValues',
											data	:	{ value : value, type : $this[0].id, pageMode : pageMode },
											dataType:	"json",
											success	:	function(data) {
															callback(data);
														}
										});
									}
				});
			}
		});
	}

	return {
		bindDependentDropdowns			:	bindDependentDropdowns,
		PageMode						:	PageMode
	};
}());