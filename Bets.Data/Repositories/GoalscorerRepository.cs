using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class GoalscorerRepository : Repository<Player, GoalscorerModel>
	{
		public GoalscorerRepository() { }
        public GoalscorerRepository(int userID) : base(userID) { }
        public GoalscorerRepository(BetsDataContext context, int userID) : base(context, userID) { }

		public override IQueryable<IModel> GetListingItems(ListingParams<GoalscorerModel> listingDataModel)
		{
			if (listingDataModel.Model.BetExpired)
				return this.GetSelectedGoalscorer(listingDataModel);
			else
				return this.GetGoalscorersForListing(listingDataModel);
		}

		public IQueryable<IModel> GetSelectedGoalscorer(ListingParams<GoalscorerModel> listingDataModel)
		{
			return this.GetGoalscorersForListing(listingDataModel).Where(e => e.BetMade);
		}

        public IQueryable<GoalscorerModel> GetGoalScorerForUserBets(int userID)
        {
            return
            (
                from	globalBet in this.Context.GlobalBets
						join
						goalscorer in this.Context.Players on globalBet.GoalscorerID equals goalscorer.PlayerID
						join
						team in this.Context.Teams on goalscorer.TeamID equals team.TeamID
                where	globalBet.UserID == userID
                select	new GoalscorerModel
						{
							ID = goalscorer.PlayerID,
							Name = goalscorer.Name,
							TeamFlag = team.FlagPrefix,
							GoalsScored = goalscorer.GoalsScored,
							ExternalID = goalscorer.ExternalID,
							Points = goalscorer.Points,
							BetMade = (goalscorer.PlayerID == globalBet.GoalscorerID)
						}
            );
        }

		public IQueryable<GoalscorerModel> GetGoalscorersForListing(ListingParams<GoalscorerModel> listingDataModel)
		{
			var models =
			(
				from	player in this.GetAll()
						join
						team in this.Context.Teams on player.TeamID equals team.TeamID
						join
							allBet in this.Context.GlobalBets on this.UserID equals allBet.UserID into allBets
							from bet in allBets.DefaultIfEmpty()
				orderby	player.GoalsScored descending, player.Points ascending, player.Name
				select	new GoalscorerModel
						{
							ID = player.PlayerID,
							Name = player.Name,
							TeamFlag = team.FlagPrefix,
							GoalsScored = player.GoalsScored,
							ExternalID = player.ExternalID,
							Points = player.Points,
                            BetMade = (bet != null && bet.GoalscorerID != null && player.PlayerID == bet.GoalscorerID)
                        }
			);

			if (listingDataModel != null && !string.IsNullOrEmpty(listingDataModel.Keyword))
				models = models.Where(e => e.Name.Contains(listingDataModel.Keyword));

			return models;
		}

		public override ActionStatus SaveItem(GoalscorerModel model, DBActionType action)
		{
            //add bet only if global bets are not expired
            if (new MatchRepository().GlobalBetsExpired())
            {
                return new ActionStatus
                {
                    Success = false,
                    Message = "Bets on the goalscorer are not allowed anymore"
                };
            }

            return this.CallStoredProcedure
			(
				action,
				() => {	return new SPResult { Result = this.Context.SaveGoalscorerBet(this.UserID, model.ID) };	}
			);
		}
	}
}
