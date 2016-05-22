var AdditionalMethods = (function() {

	///<summary>RequiredIf</summary>
	function addRequiredIf() {

		$.validator.addMethod('requiredif',
			function (value, element, parameters) {

				var id = '#' + parameters['dependentproperty'],

					// get the target value (as a string, as that's what actual value will be)
					targetvalue = parameters['targetvalue'],

					// get the actual value of the target control
					$control = $(id),
					controltype = $control.attr('type'),
					actualvalue = (controltype === 'checkbox') ? $control.attr('checked').toString() : $control.val();

				targetvalue = (targetvalue == null ? '' : targetvalue).toString();

				//when dependent dropdown is changed, revalidate
				$control.change(function() {
					$('.frm-detail').validate().element($(element));
				});

				// if the condition is true, reuse the existing required field validator functionality
				if (targetvalue.toLowerCase() === actualvalue.toLowerCase())
					return $.validator.methods.required.call(this, value, element, parameters);

				return true;
			}
		);

		$.validator.unobtrusive.adapters.add('requiredif', ['dependentproperty', 'targetvalue'],  function (options) {
			options.rules['requiredif'] = {
				dependentproperty: options.params['dependentproperty'],
				targetvalue: options.params['targetvalue']
			};
			options.messages['requiredif'] = options.message;
		});
	}

	///<summary>RequiredIf</summary>
	function addRequiredIfNot() {

		$.validator.addMethod('requiredifnot',
			function (value, element, parameters) {

				var id = '#' + parameters['dependentproperty'],

					// get the target value (as a string, as that's what actual value will be)
					targetvalue = parameters['targetvalue'],

					// get the actual value of the target control
					$control = $(id),
					controltype = $control.attr('type'),
					actualvalue = (controltype === 'checkbox') ? $control.attr('checked').toString() : $control.val();

				targetvalue = (targetvalue == null ? '' : targetvalue).toString();

				//when dependent dropdown is changed, revalidate
				$control.change(function() {
					$('.frm-detail').validate().element($(element));
				});

				// if the condition is true, reuse the existing required field validator functionality
				if (targetvalue.toLowerCase() !== actualvalue.toLowerCase())
					return $.validator.methods.required.call(this, value, element, parameters);

				return true;
			}
		);

		$.validator.unobtrusive.adapters.add('requiredifnot', ['dependentproperty', 'targetvalue'],  function (options) {
			options.rules['requiredifnot'] = {
				dependentproperty: options.params['dependentproperty'],
				targetvalue: options.params['targetvalue']
			};
			options.messages['requiredifnot'] = options.message;
		});
	}

	addRequiredIf();
	addRequiredIfNot();
}());

///<summary>
/// Bind dynamic unobtrusive validation
/// http://xhalent.wordpress.com/2011/01/24/applying-unobtrusive-validation-to-dynamic-content/
///</summary>
(function ($) {

	$.validator.unobtrusive.parseDynamicContent = function(selector) {

		//use the normal unobstrusive.parse method
		$.validator.unobtrusive.parse(selector);

		//get the relevant form
		var form = $(selector).first().closest('form');
  
		//get the collections of unobstrusive validators, and jquery validators
		//and compare the two
		var unobtrusiveValidation = form.data('unobtrusiveValidation');
		var validator = form.validate();

		$.each(unobtrusiveValidation.options.rules, function(elname, elrules) {

			if (validator.settings.rules[elname] == undefined) {
				var args = {};
				$.extend(args, elrules);
				args.messages = unobtrusiveValidation.options.messages[elname];
				//edit:use quoted strings for the name selector
				$("[name='" + elname + "']").rules("add", args);
			} 
			else {
				$.each(elrules, function (rulename, data) {

					if (validator.settings.rules[elname][rulename] == undefined) {
						var args = {};
						args[rulename] = data;
						args.messages = unobtrusiveValidation.options.messages[elname][rulename];
						//edit:use quoted strings for the name selector
						$("[name='" + elname + "']").rules("add", args);
					}
				});
			}
		});
	}
})($);