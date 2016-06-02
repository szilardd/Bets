var AdminPage = (function() {

	var $container;

	var Match = function(name) {
		this.RoundID = ko.observable();
		this.FirstTeamID = ko.observable();
		this.SecondTeamID = ko.observable();
		this.Date = ko.observable();
		this.Points1 = ko.observable();
		this.PointsX = ko.observable();
		this.Points2 = ko.observable();
	};

	var addMatchViewModel = {
		matches		:	ko.observableArray(),
		addMatch	:	function() {
							this.matches.push(new Match());
							Bootstrapper.bindDropdowns($container);
							Bootstrapper.bindDatePickers('.Date');
						},
		removeMatch	:	function(item) {

							if (this.matches().length <= 1)
								return;

							this.matches.remove(item);
							Bootstrapper.bindDropdowns($container);
							Bootstrapper.bindDatePickers('.Date');
						},

		save		:	function() {

							var $this = $(this),
								$block = $this.parent();

							Utils.blockElement($block, true);

							AjaxUtils.post({
								url			:	"Admin/AddMatches",
								json		:	true,
								dataType	:	"json",
								data		:	ko.toJSON({ matches: addMatchViewModel.matches }),
								success		:	function(result) {
													endAction(result, $this);
												},
								error		:	function() {
													endAction(false, $this);
												}
							});
						}
	};

	function endAction(status, $trigger) {

		if (!status)
			$trigger.next().removeClass().addClass('error-icon');
		else
			$trigger.next().removeClass().addClass('success-icon');

		Utils.unblockElement($trigger.parent(), true);
	}

	function bindAddMatch() {

		ko.bindingConventions.conventions("#admin-content", {
			"#admin-content"		:	{ 'with': addMatchViewModel },
			".match-list"			:	function(addMatchViewModel) { return { foreach: addMatchViewModel.matches }; },
			".RoundID"				:	function(match) { return { value: match.RoundID }; },
			".FirstTeamID"			:	function(match) { return { value: match.FirstTeamID }; },
			".SecondTeamID"			:	function(match) { return { value: match.SecondTeamID}; },
			".Date"					:	function(match) { return { value: match.Date}; },
			".Points1"				:	function(match) { return { value: match.Points1 }; },
			".PointsX"				:	function(match) { return { value: match.PointsX} ; },
			".Points2"				:	function(match) { return { value: match.Points2 }; },
			".btnAddMatch"			:	{ click: function() { addMatchViewModel.addMatch(); } },
			".btnRemoveMatch"		:	{ click: function() { addMatchViewModel.removeMatch(this); } }
		});

		ko.applyBindings(addMatchViewModel);
		addMatchViewModel.addMatch();

		$container.find('.btn-add-matches').click(addMatchViewModel.save);
	}

	function bindAddMatchResult() {

		$container.find('.btn-add-match-result').click(function() {

			var $this = $(this),
				$block = $this.parent(),
				$matchID = $('#MatchID');

			Utils.blockElement($block, true);

			AjaxUtils.post({
				url		:	'Admin/AddMatchResult',
				data	:	{ ID : $matchID.val(), FirstTeamGoals : $('#FirstTeamGoals').val(), SecondTeamGoals: $('#SecondTeamGoals').val() },
				dataType:	"json",
				success	:	function(result) { 

								//remove match from list
								/*if (result) {
									$matchID.find(':selected').remove();
									$matchID.trigger("liszt:updated");
								}*/

								endAction(result, $this);
							},
				error	:	function(result) { endAction(false, $this); }
			});
		});
	}



	function bindAddPlayerGoal() {

		var $goalContainer = $container.find('.goal-container'),
			$goalsScored = $('#GoalsScored'),
			$goalsScoredTotal = $('#GoalsScoredTotal'),
			$playerID = $('#PlayerID');

		//restrict input
		$goalsScored.mousedown(function () {
			return false;
		});

		//set goals when changing player
		$playerID.change(function () {

			var $option = $(this).find(':selected');

			$goalsScored.val($option.data('custom') || 0);
			$goalsScoredTotal.text($option.data('custom2') || 0);
		});

		//set goals scored
		$goalContainer.delegate('.btn-goal', 'click', function () {

			var $this = $(this),
				val = parseInt($this.data('val')),
				newValue = parseInt($goalsScored.val()) + val;

			//set total goals
			if (newValue >= 0 || val == 1)
				$goalsScoredTotal.text(parseInt($goalsScoredTotal.text()) + val);

			if (!newValue || newValue < 0)
				newValue = 0;

			//set goals
			$goalsScored.val(newValue);
		});

		//save goals
		$goalContainer.find('.btn-save-goals').click(function () {

			var $this = $(this),
				$block = $this.parent();

			Utils.blockElement($block, true);

			AjaxUtils.post({
				url		:	'Admin/AddGoalscorerForRound',
				data	:	{ GoalscorerID : $('#PlayerID').val(), Goals: $('#GoalsScored').val() },
				dataType:	"json",
				success	:	function(result) { 
								
								//update goals scored
								if (result) {

									$playerID.find(':selected')
										.data('custom', $goalsScored.val())
										.data('custom2', $goalsScoredTotal.text());
								}

								endAction(result, $this);
							},
				error	:	function(result) { endAction(false, $this); }
			});
		});
	}

	function bindAddRound() {

		$container.find('.btn-add-round').click(function () {

			var $this = $(this),
				$block = $this.parent();

			Utils.blockElement($block, true);

			AjaxUtils.post({
				url		:	'Admin/AddRound',
				data	:	{ Name : $('#RoundName').val() },
				dataType:	"json",
				success	:	function(result) { 
								endAction(result, $this);
							},
				error	:	function(result) { endAction(false, $this); }
			});
		});
	}

	function bindRemoveTeam() {

		$container.find('.btn-remove-team').click(function() {

			var $this = $(this),
				$block = $this.parent(),
				$teamID = $('#RemoveTeamID');

			Utils.blockElement($block, true);

			AjaxUtils.post({
				url		:	'Admin/RemoveTeam',
				data	:	{ ID : $teamID.val() },
				dataType:	"json",
				success	:	function(result) { 

								//remove match from list
								if (result) {
									$teamID.find(':selected').remove();
									$teamID.trigger("liszt:updated");
								}

								endAction(result, $this);
							},
				error	:	function(result) { endAction(false, $this); }
			});
		});
	}

	function bindRemovePlayer() {

		$container.find('.btn-remove-player').click(function() {

			var $this = $(this),
				$block = $this.parent(),
				$playerID = $('#RemovePlayerID');

			Utils.blockElement($block, true);

			AjaxUtils.post({
				url		:	'Admin/RemovePlayer',
				data	:	{ ID : $playerID.val() },
				dataType:	"json",
				success	:	function(result) { 

								//remove match from list
								if (result) {
									$playerID.find(':selected').remove();
									$playerID.trigger("liszt:updated");
								}

								endAction(result, $this);
							},
				error	:	function(result) { endAction(false, $this); }
			});
		});
	}

	function bindCloseRound() {

		$container.find('.btn-close-round').click(function () {

			var $this = $(this),
				$block = $this.parent();

			Utils.blockElement($block, true);

			AjaxUtils.post({
				url		:	'Admin/CloseCurrentRound',
				dataType:	"json",
				success	:	function(result) { 
								endAction(result, $this);
							},
				error	:	function(result) { endAction(false, $this); }
			});
		});
	}

	function bindGetMatches() {

	    var $this = $(this),
				    $block = $this.parent();

	    $container.find('.btn-get-matches').click(function () {

	        Utils.blockElement($block, true);

	        AjaxUtils.post({
	            url: 'Admin/AddMatch',
	            data: { roundID: $("#RoundForGet").val() },
	            dataType: "json",
	            success: function (result) {
	                endAction(result, $this);
	            },
	            error: function (result) { endAction(false, $this); }
	        });
	    });
	}

	function bindSendEmails() {

	    var $this = $(this),
            $block = $this.parent(),
            $message = $('.message');

	    $container.find('.btn-send-email').click(function () {

	        $message.text('Wait for it...');
	        Utils.blockElement($block, true);

	        AjaxUtils.post({
	            url: 'Admin/SendEmail',
	            data: { email: $("#Email").val() },
	            dataType: "json",
	            success: function (result) {

	                if (result) {
	                    $message.text(result.Message);
	                }
	                else {
	                    $message.text('Unknown error');
	                }

	                endAction(result, $this);
	            },
	            error: function (result) {

	                if (result) {
	                    $message.text(result.Message);
	                }
	                else {
	                    $message.text('Unknown error');
	                }

	                endAction(false, $this);
	            }
	        });
	    });

	}

	function init() {

	    $container = $('#admin-content');

	    bindAddMatch();
	    bindAddMatchResult();
	    bindAddPlayerGoal();
	    bindAddRound();
	    bindRemoveTeam();
	    bindCloseRound();
	    bindRemovePlayer();
	    bindGetMatches();
	    bindSendEmails();
	}

	return {
		init : init
	};

}());

$(function() { AdminPage.init(); });