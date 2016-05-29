var MatchesForRoundPage = (function() {

	var userBonus,
		maxBonusPerMatch;

	var MatchViewModel = function(params) {
		this.id = params.matchID;
		this.firstTeamGoals = ko.observable(params.firstTeamGoals);
		this.secondTeamGoals = ko.observable(params.secondTeamGoals);
		this.bonus = ko.observable(parseInt(params.bonus || 0));
	};

	var MatchBetViewModel = function(params) {

		var self = this;

		this.$page = params.$page;
		this.$row = params.$row;
		this.$betRow = params.$betRow;
		this.$btnTrigger = params.$btnTrigger;

		this.matchViewModel = new MatchViewModel(params);
		this.savedMatchViewModel = new MatchViewModel(params);

		this.maxBonusPerMatch = maxBonusPerMatch;
		this.maxUserBonus = ko.observable(Math.min(parseInt(userBonus) + this.matchViewModel.bonus(), maxBonusPerMatch));

		this.save = function () {

			var $btnSave = self.$betRow.find('.btn-save').parent();

			//validate
			if (self.matchViewModel.firstTeamGoals() == '' || self.matchViewModel.secondTeamGoals() == '') {
				self.$betRow.addClass("invalid");
				return;
			}

			//inline block
			Utils.blockElement($btnSave, true);

			AjaxUtils.post({
				url				:	self.$page.data('pagetype') + '/Add',
				json			:	true,
				data			:	ko.toJSON(self.matchViewModel),
				dataType		:	"json",
				success			:	function (response) {

										//refresh user bonus
										userBonus = parseInt(response.UserBonus);
										self.maxUserBonus(Math.min(self.matchViewModel.bonus() + userBonus, maxBonusPerMatch));

										if (response.Success === false)
											self.$betRow.addClass("invalid");
										else
											self.endEdit();

										Utils.unblockElement($btnSave);

                                        // update listing row UI
										var $rowBonusElement = self.$row.find('.bonus-single');
										if (self.matchViewModel.bonus()) {
										    $rowBonusElement.removeClass('hidden');
										}
										else {
										    $rowBonusElement.addClass('hidden');
										}
									},
				error			:	function () {
										self.$betRow.addClass("invalid");

										Utils.unblockElement($btnSave);
									}
			});
		};

		this.saveOnEnter = function(model, event) {

			//enter
			if (event.which == 13)
				model.save();

			return true;
		};

		this.endEdit = function () {
			
			//reset UI
			Utils.unblockElement(self.$betRow.find('.btn-save').parent());
			self.$betRow.addClass('hide').removeClass('invalid');
			self.$btnTrigger.removeClass('hide');

			//set changes in saved match
			self.savedMatchViewModel.bonus(self.matchViewModel.bonus());
			self.savedMatchViewModel.firstTeamGoals(self.matchViewModel.firstTeamGoals());
			self.savedMatchViewModel.secondTeamGoals(self.matchViewModel.secondTeamGoals());

			//hide warning
			self.$row.find('.bet-warning').replaceWith($("<span>", { html: "&nbsp;-&nbsp;" }));
		};

		this.updateBonusPoints = function () {
			
			var $this = $(this),
				val = $this.data('val'),
				newValue = parseInt(self.matchViewModel.bonus() + parseInt(val));

			if (!newValue || newValue < 0)
				newValue = 0;

			if (newValue > self.maxUserBonus())
				newValue = self.maxUserBonus();

			if (newValue > maxBonusPerMatch)
				newValue = maxBonusPerMatch;
			
			self.matchViewModel.bonus(newValue);
		};
	};

	function init() {

		var betTemplate;

		maxBonusPerMatch = $('#fld-max-bonus').val();
		userBonus = $('#UserBonus').val();
		betTemplate = $("#bet-template").html();

		//show bet container when clicking on bet button
		$('#listing-matchesforcurrentround,#listing-matchesfornextround').delegate('.btn-bet', 'click', function () {

			//if another bet row is shown, hide it
			$('.btn-bet').removeClass('hide');
			$('tr[class*=bet-row]').remove();

			var	$this = $(this),
				$row = $this.parents('tr:first'),
				$page = $this.parents('.listing-page:first');

			//hide bet button
			$this.addClass('hide');

			//make sure to bind only once
			if ($row.next().hasClass('hide')) {
				$row.next().removeClass('hide');
				return;
			}

			var	matchID = $row.data('id'),
				$betRow = $(betTemplate).addClass('bet-row-' + matchID),
				betViewModel;
			
			//add bet row after current row	
			$row.after($betRow);
			$row.addClass('row-' + matchID);

			//validate entered bonus
			$betRow.find('#Bonus').keyup(function (e) {

				if (e.which == 8) //backspace
					return true;

				var $this = $(this),
					val = parseInt($this.val());

				if (!val)
					val = 0;
				else if (val < 0)
					val = 0;
				else if (val > betViewModel.maxUserBonus())
					val = betViewModel.maxUserBonus();
				
				if (val > maxBonusPerMatch)
					val = maxBonusPerMatch;

				$this.val(val);
			});

			//create view model
			betViewModel = new MatchBetViewModel({
				$page			:	$page, 
				$betRow			:	$betRow, 
				$row			:	$row,
				$btnTrigger		:	$this, 
				matchID			:	matchID, 
				firstTeamGoals	:	$row.find('.FirstTeamGoals').text(),
				secondTeamGoals	:	$row.find('.SecondTeamGoals').text(),
				bonus			:	$row.find('.Bonus').text()
			});

            
			var $btnSingle = $betRow.find('.btn-bonus-single');

			if (betViewModel.bonus) {
			    $btnSingle.addClass('selected');
			}
			else {
			    $btnSingle.removeClass('selected');
			}

			//create and apply bet row bindings
			ko.bindingConventions.conventions({
				".btn-save"			:	{ 	click	:	function(viewModel, event) {
															viewModel.save();
														}
										},
				'#FirstTeamGoals'	:	function(viewModel) {
											return { value : viewModel.matchViewModel.firstTeamGoals, event : { keyup : viewModel.saveOnEnter } };
										},
				'#SecondTeamGoals'	:	function(viewModel) {
											return { value : viewModel.matchViewModel.secondTeamGoals, event : { keyup : viewModel.saveOnEnter } };
										},
				'#Bonus'			:	function(viewModel) {
											return { value : viewModel.matchViewModel.bonus, event : { keyup : viewModel.saveOnEnter } };
										},
				'.MaxUserBonus'		:	function(viewModel) {
											return { text: viewModel.maxUserBonus }
										},
				'.btn-bonus'		:	{	click	:	function(viewModel, event) {
															viewModel.updateBonusPoints.call(event.target);
														}
										},
				'.btn-bonus-single':    {
                                            click   :   function(viewModel, event) {

                                                            var $element = $(event.target);
                                                            $element.toggleClass('selected');

                                                            var bonus = $element.hasClass('selected') ? 1 : 0;
                                                            viewModel.matchViewModel.bonus(bonus);

                                                            $row.find('.Bonus').text(bonus);
                                                        }
                        				}
			});

			ko.applyBindings(betViewModel, $betRow[0]);

			//create and apply listing row bindings			
			ko.bindingConventions.conventions({
				'.FirstTeamGoals'	:	function(viewModel) {
											return { text : viewModel.firstTeamGoals };
										},
				'.SecondTeamGoals'	:	function(viewModel) {
											return { text : viewModel.secondTeamGoals };
										}
			});

			ko.applyBindings(betViewModel.savedMatchViewModel, $row[0]);

			//set focus on first team
			$betRow.find('#FirstTeamGoals').focus();
		});

		Bootstrapper.bindMatchView('MatchesForCurrentRound');
	}

	return {
		init	:	init
	};
}());

$(function() { MatchesForRoundPage.init(); })