var AjaxUtils;

///<summary>
/// Wraps jQuery ajax calls (ajax, get, post, load) and provides:
/// 1. Default success and error callbacks
/// 2. Element blocking during the AJAX call (using the blockUI jQuery plugin: http://jquery.malsup.com/block/)
///</summary>
AjaxUtils = (function() {
	
	var ajaxConfig = {
		dataType: "text",
		type: "POST",
		defaultErrorHandling: true, //call defaultErrorCallback in case of error and no error callback is given
		defaultSuccessHandling: false //calls defaultSuccessCallback in case of success and no success callback is given
	};

	///<summary>Default error callback. If no error callback is supplied, this will be called on error</summary>
	function defaultErrorCallback(data) {
		Utils.closeModal();
	}

	///<summary>Default success callback. If no success callback is supplied, this will be called on error</summary>
	function defaultSuccessCallback(xhr) {
	}

	///<summary>
	/// Sets all parameters required for an ajax call + additional parameters can be supplied, as follows:
	/// - blockElement: which jQuery element should be blocked during the ajax call (with blockUi)
	/// - blockSelector: jQuery selector for blocking
	///	- success: success callback. If provided, this will be cloned and wrapped inside a new callback which automatically
	///			   unblocks UI elements if blockElement or blockSelector was provided
	/// - error: error callback. Same behavior as success callback;
	/// - other parameters: any parameter which can be supplied to $.ajax
	///</summary>
	function setParameters(parameters) {

		parameters = $.extend({}, ajaxConfig, parameters);

		//prepend base url
		parameters.url = Config.BaseURL + parameters.url;
		
		//send anti forgery token
		if (parameters.sendToken) {
			parameters.data = parameters.data || {};
			$.extend(parameters.data, Utils.getAntiForgeryToken());
		}

		//if error callback is provided, remove default error handling
		if (parameters.error)
			parameters.defaultErrorHandling = false;

		if (parameters.json === true)
			parameters.contentType = "application/json; charset=utf-8"; //json by default
		
		//determine element to be blocked with blockUI during AJAX call
		var $blockElement = parameters.blockElement, newSuccessCallback, newErrorCallback;
		if (!$blockElement && parameters.blockSelector)
			$blockElement = $(parameters.blockSelector);

		if ($blockElement && $blockElement.length > 0)
			Utils.blockElement($blockElement);

		//clone success calback so that it can be called inside the new callback function
		if (parameters.success)
			newSuccessCallback = parameters.success.clone();

		//define final success callback
		parameters.success = function(data) {

			if (data === false)
				parameters.error(data);
			else if (data && data.Success === false)
				parameters.error(data);
			else {

				if (newSuccessCallback)
					newSuccessCallback(data); //call original success callback

				if (parameters.defaultSuccessHandling) defaultSuccessCallback(data); //default success handling
			}

			if ($blockElement)
				Utils.unblockElement($blockElement);
		};

		//clone error calback so that it can be called inside the new callback function
		if (parameters.error)
			newErrorCallback = parameters.error.clone();

		//define final error callback
		parameters.error = function(data) {

			if (data.status == 401) //unauthorized
				location.reload();
			else if (data.status === 0)
				parameters.success(data.responseText);
			else {

				if (newErrorCallback) newErrorCallback(data); //call original error callback

				if ($blockElement)
					Utils.unblockElement($blockElement);

				if (parameters.defaultErrorHandling !== false)
					defaultErrorCallback(data); //default error handling
			}
		};

		//send csrf token
		if (parameters.sendToken)
			parameters.headers = Utils.getAntiForgeryToken();

		return parameters;
	}

	///<summary>Wrapper for $.ajax</summary>
	function ajax(parameters) {
		parameters = setParameters(parameters);
		return $.ajax(parameters);
	}

	///<summary>Wrapper for $.get</summary>
	function get(parameters) {
		parameters.type = "GET";
		return ajax(parameters);
	}

	///<summary>Wrapper for $.post</summary>
	function post(parameters) {

		parameters.type = "POST";
		return ajax(parameters);
	}

	///<summary>Wrapper for $.load</summary>
	function load($element, parameters) {
		parameters = setParameters(parameters);
		$element.load(parameters.url, parameters.data, parameters.success);
	}

	//public functions
	return {
		ajax: ajax,
		load: load,
		post: post,
		get: get
	};
} ());

//register error handler for external AJAX
$.ajaxSetup({
	error :	function(jqXHR, textStatus, errorThrown) {
				if (jqXHR.status == 401) //access denied
					location.reload();
			}
});