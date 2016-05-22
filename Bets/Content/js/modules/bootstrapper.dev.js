var Bootstrapper = (function() {
	
	function bindDropdowns(selector) {

		var $container = !selector ? $('select') : $(selector).find('select');
		$container.chosen();
	}

	function bindDatePickers(selector) {

		$(selector).datetimepicker({
			dateFormat	:	Config.DateFormat,
			stepMinute	:	5
		});
	}

	///<summary>Binds view function for match (bets made by other users on match)</summary>
	function bindMatchView(listingID) {

		//show bets made by other user for the current match
		$('#listing-' + listingID.toLowerCase()).delegate('.btn-view', 'click', function () {

			var	$this = $(this),
				$row = $this.parents('tr:first');

			if ($this.hasClass('active')) {
				$this.removeClass('active');
				$this.val("View");
				$row.next().remove();
			}
			else {
			
				$this.val("Close");
				$this.addClass('active');

				var matchID = $row.data('id'),
					$nextRow = $row.next(),
					$td;

				//if row already exist, remove it
				if ($nextRow.hasClass("matchbet-row"))
					$nextRow.remove();

			    //inline block
				$this.attr('disabled');

				AjaxUtils.post({
					url			:	"MatchBet?MatchID=" + matchID,
					success		:	function(html) {

										$row.after($("<tr>", { "class" : "matchbet-row" }));
										$nextRow = $row.next();

										$nextRow.append($("<td>", { colspan: 13 }));
										$td = $nextRow.find(':first');

										$td.html(html);

										$this.removeAttr('disabled');
									}
				});
			}
		});
	}

	function init() {
		//bindDropdowns();
	}

	return {
		init			:	init,
		bindDropdowns	:	bindDropdowns,
		bindDatePickers	:	bindDatePickers,
		bindMatchView	:	bindMatchView
	};

}());

$(function() { Bootstrapper.init(); });