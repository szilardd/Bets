var RoundsPage = (function() {

	var $slider = $('#round-slider'),
		$matches = $('#round-matches'),
		$bestUsers = $('#listing-bestusersforround').parent(),
		$worstUsers = $('#listing-worstusersforround').parent(),
		firstLoad = true;

	function bindListing(type) {

		var listing = new ListingPage({ listingType : type });
		listing.init();
	}

	function getBets(roundID) {

		//set timeout to make sure that active class is set to the current slider item
		setTimeout(function() {

			getListingDataForRound(roundID, "MatchesForRound", $matches);
			getListingDataForRound(roundID, "BestUsersForRound", $bestUsers);
			getListingDataForRound(roundID, "WorstUsersForRound", $worstUsers);
		}, 200);
	}

	function getListingDataForRound(roundID, type, $container) {

		type = type.toLowerCase();
			
		AjaxUtils.post({
			blockElement:	$container,
			url			:	type,
			data		:	{ RoundID : roundID },
			success		:	function(html) {
								$container.html(html);
								bindListing(type);

								//bind view match functionality
								if (type == "matchesforround")
									Bootstrapper.bindMatchView("MatchesForRound");
							}
		});
	}

	function clearListings() {
		$('.listing-page').remove();
	}

	function init() {

		//bind first listing
		bindListing("MatchesForRound");

		//bind view match functionality
		Bootstrapper.bindMatchView("MatchesForRound");
		
		//create slider
		$slider.rhinoslider({
			controlsPlayPause	:	false,
			showCaptions		:	'always',
			showBullets			:	'never',
			effectTime			:	300,
			cycled              :   false,
			slideNextDirection  :   'toLeft',
			slidePrevDirection  :   'toRight'
		});

		//set first slide as active
		$slider.find('.slide:first').addClass('active');

		$slider.delegate(".slide", "click", function () {

			var $this = $(this);
			getBets($this.data('id'));

			//set current slide as active
			$slider.find('.slide').removeClass('active');
			$this.addClass('active')
		});
	}
	
	return {
		init : init
	};
}());

$(function() { RoundsPage.init(); });