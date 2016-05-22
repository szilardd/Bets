///<summary>Object which provides the following utils:
/// - blocking UI elements
/// - encryption
/// - binding iScroll (for scrolling capability on mobile devices)
/// - date formatting
/// - logging
///</summary>
var Utils = (function() {

	var config = {
		blockUI		:	{
							message: '<div class="ajax-loader"><img src="' + Config.BaseURL + 'content/img/ajax-loader.gif"/></div>',
							css: { border: '0px', 'background-color': 'transparent', 'z-index': '999999' },
							overlayCSS: { opacity: 0.01, cursor: 'pointer' }
						}, //blockUI style and progress image
		fancybox	:	{
							hideOnOverlayClick	: false, //disables overlay closing when the overlay is clicked
							hideOnContentClick	: false, //disables overlay closing when the content outside the overlay is clicked
							titleShow			: false,
							titlePosition		: 'inside',
							overlayOpacity		: 1,
							overlayColor		: '#000',
							transitionOut     	: 'fade',
							transitionIn      	: 'fade',
							easingIn          	: 'swing',
							easingOut         	: 'swing',
							speedIn           	: 600,
							speedOut          	: 600
						}
	};
	
	var index = 1;

	/* BLOCK UI */
	function blockSelector(selector) {
		blockElement($(selector));
	}

	function unblockSelector(selector) {
		unblockElement($(selector));
	}

	function blockElement($element, inline) {
		
		var params;		

		//if element is already blocked, do nothing
		if ($element.data('blockUI.isBlocked') == 1)
			return;

		//if inline, display small loading progress
		if (inline) {
			var message = { 
				message : config.blockUI.message.replace('ajax-loader', 'ajax-loader small').replace('ajax-loader.gif', 'ajax-loader-small.gif'),
				overlayCSS : { opacity: 0.2 }
			};

			params = $.extend({}, config.blockUI, message);
		}
		else
			params = config.blockUI;

		if ($element.length > 1)
			$element = $($element[0]);

		switch ($element.selector) {
		
			case "body": { $.blockUI(params); break; } //if selector is body, block the whole page
			default: $element.block(params);
		}
	}
	
	function unblockElement($element) {
		
		if ($element.selector === "body") //if selector, unblock the whole page
			$.unblockUI();
		else
			$element.unblock();
	}
	
	/* FANCYBOX */
	
	function confirm(parameters) {

		var result = false,
			text = '';

		new TemplateModule({
			name		:	"ConfirmModal",
			url			:	"User/GetConfirmModal",
			data		:	{ hasInput : parameters.hasInput, message : parameters.message },
			callback	:	function(content) {
								
								modal({
									content				:	content,
									onComplete			:	function() {

																var $container = $(content);

																$("#confirm-cancel").click(function() {
																	result = false;
																	$.fancybox.close();
																});
				
																$("#confirm-ok").click(function() {

																	if (parameters.hasInput) {
																		text = $.trim($("#confirm-text").val());
																		if (text === "")
																			return;
																	}

																	result = true;

																	$.fancybox.close();
																});
															},
									onClosed			:	function() {
																
																if (parameters.callback)
																	parameters.callback.call(this, result, text);
															}
								});
							}
		}).renderDataTemplate();
	}

	///<summary>
	/// Shows fancybox automatically (without trigger)
	///</summary>
	function modal(parameters) {

		$.fancybox($.extend(config.fancybox, {
			content		:	parameters.content,
			onComplete	:	parameters.onComplete,
			onClosed	:	parameters.onClosed
		}));
	}

	function closeModal() {
		$.fancybox.close();
	}

	/* DATE */

	function addDateField(elementSelector, datePickerCloseEvent) {
		
	   $(elementSelector).datepicker
	   ({ 
	        showOn: 'button', 
	        dateFormat: Config.DatePickerFormat,
	        buttonImageOnly: true, 
	        buttonImage: Config.BaseURL + 'content/img/calendar.png',
	        css: {'z-index':'1002'},
	        onClose: datePickerCloseEvent
	   }); 
	   
	   restrictKeyboardAction(elementSelector);
	}
	
	function restrictKeyboardAction(elementSelector) {
		
	    $(elementSelector)
		    .keypress
		    (
		        function(e)
		        {
		            if (e.which != 0) // 0 = TAB
		                return false;
		        }
		    )
		    .click
		    (
		        function()
		        {
		            this.blur();                 // most browsers 
		            this.hideFocus = true;       // internet explorer
		            this.style.outline = 'none'; // mozilla
		        }
		    );
	}

	function dateToISO(date) {

		if (date == null)
			return null;

		return date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
	}

	function convertISODateToObject(date) {

		if (date == "")
			return null;

		return Date.parse(date, "yyyy-MM-dd HH:mm:ss");
	}


	/* OTHER */
	
	function getAntiForgeryToken() {
		return { __RequestVerificationToken : $("[name=__RequestVerificationToken]:first").val() };
	}

	return {

		blockElement			:	blockElement,
		blockSelector			:	blockSelector,
		unblockElement			:	unblockElement,
		unblockSelector			:	unblockSelector,

		confirm					:	confirm,
		modal					:	modal,
		closeModal				:	closeModal,

		getAntiForgeryToken		:	getAntiForgeryToken,
		addDateField			:	addDateField,
		dateToISO				:	dateToISO,
		convertISODateToObject	:	convertISODateToObject
	};

} ());