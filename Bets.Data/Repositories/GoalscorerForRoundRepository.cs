using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class GoalscorerForRoundRepository : Repository<Player, GoalscorerModel>
	{
		public GoalscorerForRoundRepository() : base() { }
		public GoalscorerForRoundRepository(int userID) : base(userID) {}

		public override IQueryable<IModel> GetListingItems(ListingParams<GoalscorerModel> listingDataModel)
		{
			var models =
			(
				from	player in this.GetAll()
				from	setting in this.Context.Settings
						join 
						match in this.Context.Matches on setting.CurrentRoundID equals match.RoundID
						join
						team in this.Context.Teams on player.TeamID equals team.TeamID
				where	player.Active && (player.TeamID == match.FirstTeamID || player.TeamID == match.SecondTeamID)
				orderby player.GoalsScored descending, player.Points ascending, player.Name
				select new GoalscorerModel
						{
							ID = player.PlayerID,
							Name = player.Name,
							TeamFlag = team.FlagPrefix,
							GoalsScored = player.GoalsScored,
							Points = Convert.ToInt32(player.Points * setting.RoundGoalMultiplier),
							ExternalID = player.ExternalID
						}
			);


			if (!String.IsNullOrEmpty(listingDataModel.Keyword))
				models = models.Where(e => e.Name.Contains(listingDataModel.Keyword));

			return models;
		}

		public GoalscorerModel GetGlobalGoalscorerBet()
		{
 			return
			(
				from	player in this.GetAll()
						join
						bet in this.Context.GlobalBets on player.PlayerID equals bet.GoalscorerID
						join
						team in this.Context.Teams on player.TeamID equals team.TeamID
				where	bet.UserID == this.UserID 
				select	new GoalscorerModel
						{
							ID = player.PlayerID,
							Name = player.Name,
							TeamFlag = team.FlagPrefix,
							GoalsScored = player.GoalsScored,
							ExternalID = player.ExternalID,
                            Points = player.Points,
							BetMade = true
						}
			).FirstOrDefault();
		}

		public override ActionStatus SaveItem(GoalscorerModel model, DBActionType action)
		{
			//add bet only if round is not expired
			if (new MatchRepository().CurrentRoundIsExpired())
				return new ActionStatus();

			return this.CallStoredProcedure
			(
				action,
				() => { return new SPResult { Result = this.Context.SaveGoalscorerForRoundBet(this.UserID, model.ID) }; }
			);
		}
	}
}
