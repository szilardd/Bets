var SingleBetPage = (function () {

	function init(type) {

		var $container = $('#listing-' + type + ' .listing-items'),
			listing = ListingPages[type];

		$container.delegate('.btn', 'click', function () {

			AjaxUtils.post({
				blockElement	:	$container,
				url				:	type + '/Add',
				data			:	{ id : listing.getListingItemID($(this)) },
				success			:	function () {

										listing.reload();

										var $single = $('.' + type + '-selected');

										AjaxUtils.post({
											blockElement	:	$single,
											url				:	'MyBets/Get' + type + 'Bet',
											success			:	function(content) {
																	$single.html(content);
																}
										});
									}
			});
		});
	}

	return {
		init : init
	};
}());

$(function () { 
	SingleBetPage.init('goalscorer'); 
	SingleBetPage.init('winner');
	SingleBetPage.init('goalscorerforround');
	SingleBetPage.init('goalscorerstandings');
});