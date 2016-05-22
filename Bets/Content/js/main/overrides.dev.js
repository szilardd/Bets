function addPrototypes() {
	
	///<summary>Extends JavaScript Function object with cloning function</summary>
	Function.prototype.clone = function () {
	    var fct = this;
	    var clone = function () {
	        return fct.apply(this, arguments);
	    };
	    clone.prototype = fct.prototype;
	    for (property in fct) {
	        if (fct.hasOwnProperty(property) && property !== 'prototype') {
	            clone[property] = fct[property];
	        }
	    }
	    return clone;
	};
	
	String.prototype.ltrim = function ltrim(chars) {
		chars = chars || "\\s";
		return this.replace(new RegExp("^[" + chars + "]+", "g"), "");
	};
	
	String.prototype.rtrim = function rtrim(chars) {
		chars = chars || "\\s";
		return this.replace(new RegExp("[" + chars + "]+$", "g"), "");
	};
	
	String.prototype.trim = function(chars) {
		
		var result = this.ltrim(chars);
		return result.rtrim(chars);
	};
}

function bindJQueryPlugins() {
	
	(function($) {
	    
	    $.fn.serializeObject = function()
	    {
	        var o = {};
	        var a = this.serializeArray();
	        $.each(a, function() {
	            if (o[this.name]) {
	                if (!o[this.name].push) {
	                    o[this.name] = [o[this.name]];
	                }
	                o[this.name].push(this.value || '');
	            } else {
	                o[this.name] = this.value || '';
	            }
	        });
	        return o;
	    };
	})(jQuery);
}

addPrototypes();
bindJQueryPlugins();

jQuery.ajaxSettings.traditional = true;
