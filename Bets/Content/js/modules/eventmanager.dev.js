 /// <reference path="../jquery/jquery-1.6.4.min.js"/>

///<summary>Provides functionality for binding and triggering events (callbacks) and passing data to it</summary>
var EventManager = (function() {

	var events = {},
		//type of events which are supported by the event manager
		Event = { 
					ListingReload			: 0, 
					LookupClose				: 1, 
					LookupAdd				: 2, 
					LookupReload			: 3, 
					LookupSelected			: 4, 
					LookupItemStateChange	: 5,
					EditCancel				: 6,
					BeforeSave				: 7
				};

	///<summary>Binds event with a given name to a given callback</summary>
	function bind(eventType, name, callback) {
		
		var eventByType = events[eventType];

		if (name)
			name = name.toLowerCase();

		if (!eventByType) {
			eventByType = [];
			events[eventType] = eventByType;
		}

		if (!eventByType[name])
			eventByType[name] = [];

		eventByType[name].push(callback);
	}

	///<summary>Triggers event identified by its type and name</summary>
	function trigger(eventType, name, data, callback) {

		var eventByType = events[eventType];

		if (name)
			name = name.toLowerCase();

		if (!eventByType || !eventByType[name])
			return false;

		//trigger event
		for (var i = 0; i < eventByType[name].length; i++)
			eventByType[name][i](data, callback);

		return true;
	}

	function init() {}

	return {
		init	:	init,
		bind	:	bind,
		trigger	:	trigger,
		Event	:	Event
	};
}());

$(function() { EventManager.init(); });