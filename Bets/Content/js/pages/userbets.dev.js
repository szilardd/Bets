var UserBetsPage = (function () {

    var $userSlider = $('#userbets-slider'),
		$roundSlider = $('#round-slider'),
		$roundSliderContainer = $('#round-slider-container'),
		$matches = $('#round-matches'),
        $winnerbet = $('#winner-bets'),
        $goalscorerbet = $('#goalscorer-bets'),
        $roundgoalscorerbet = $('#round-goalscorer'),
		firstLoad = true;

    function bindListing(type) {

        var listing = new ListingPage({ listingType: type });
        listing.init();
    }

    function getBets(roundID, userID) {

        //set timeout to make sure that active class is set to the current userSlider item
        setTimeout(function () {
            getListingDataForRound(roundID, userID, "RoundMatchesForUser", $matches);
            getListingDataForRound(roundID, userID, "GoalscorerForUserBets", $goalscorerbet);
            getListingDataForRound(roundID, userID, "WinnerForUserBets", $winnerbet);
        }, 200);
    }

    function getListingDataForRound(roundID, userID, type, $container) {

        type = type.toLowerCase();

        AjaxUtils.post({
            blockElement	:	$container,
            url				:	type,
            data			:	{ RoundID: roundID, UserID: userID },
            success			:	function (html) {

									$container.html(html);
									bindListing(type);

									//bind view match functionality
									if (type == "roundmatchesforuser")
										Bootstrapper.bindMatchView('RoundMatchesForUser');
								}
        });
    }

    function init() {

        //bind first listing
        bindListing("RoundMatchesForUser");

        //create user slider
        $userSlider.rhinoslider({
            controlsPlayPause: false,
            showCaptions: 'never',
            showBullets: 'never',
            cycled: false,
            slideNextDirection: 'toLeft',
            slidePrevDirection: 'toRight'
        });

        //create round slider
        $roundSlider.rhinoslider({
            controlsPlayPause: false,
            showCaptions: 'never',
            showBullets: 'never',
            cycled: false,
            controlsMousewheel: false,
            slideNextDirection: 'toLeft',
            slidePrevDirection: 'toRight'
        });

		//set first round as default
        $roundSlider.find('.slide:first').addClass('active');

		//bind reload for user
        $userSlider.delegate(".slide", "click", function (e) {

            var $this = $(this),
				roundID = $roundSlider.find('.active:first').data('id');

            getBets(roundID, $this.data('id'));

			//show container
			$('#global-bets-container').removeClass('hide');

            $roundSliderContainer.css("visibility", "visible");

			//set current slide as active
            $userSlider.find('.slide').removeClass('active');
            $this.addClass('active');
        });

		//bind reload for round		
        $roundSlider.delegate(".slide", "click", function (e) {

            var $this = $(this),
				userID = $userSlider.find('.active:first').data('id');

            getBets($this.data('id'), userID);
            $roundSlider.find('.slide').removeClass('active');
            $this.addClass('active');
        });
    }

    return {
        init: init
    };
}());

$(function () { UserBetsPage.init(); });