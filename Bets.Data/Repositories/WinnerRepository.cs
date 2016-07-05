using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class WinnerRepository : Repository<Team, TeamModel>
	{
		public WinnerRepository() : base() { }
		public WinnerRepository(int userID) : base(userID) {}

		public override IQueryable<IModel> GetListingItems(ListingParams<TeamModel> listingDataModel)
		{
			if (listingDataModel.Model.BetExpired)
				return this.GetSelectedWinner(listingDataModel);
			else
				return this.GetWinnersForListing(listingDataModel);
		}

        public IQueryable<TeamModel> GetWinnerForUserBets(int userID)
        {
            return
            (
                from    globalBet in this.Context.GlobalBets
                        join
                        team in this.Context.Teams on globalBet.WinnerTeamID equals team.TeamID
                where   globalBet.UserID == userID
                select  new TeamModel
                        {
                            ID = team.TeamID,
                            Name = team.Name,
                            Flag = team.FlagPrefix,
                            ExternalID = team.ExternalID,
                            Points = (int)team.Points,
                            BetMade = (team != null && globalBet.WinnerTeamID != null && team.TeamID == globalBet.WinnerTeamID)
                        }
            );
        }

		public IQueryable<TeamModel> GetSelectedWinner(ListingParams<TeamModel> listingDataModel)
		{
			return this.GetWinnersForListing(listingDataModel).Where(e => e.BetMade);
		}

		public IQueryable<TeamModel> GetWinnersForListing(ListingParams<TeamModel> listingDataModel)
		{
			var models = 
			(
				from	team in this.GetAll()
                        join
                            allBet in Context.GlobalBets on UserID equals allBet.UserID into allBets
                            from globalBet in allBets.DefaultIfEmpty()
				orderby team.Points, team.Name
				select	new TeamModel
						{
							ID = team.TeamID,
							Name = team.Name,
							Flag = team.FlagPrefix,
                            Points = (int)team.Points,
							ExternalID = team.ExternalID,
                            BetMade = (globalBet != null && globalBet.WinnerTeamID != null && team.TeamID == globalBet.WinnerTeamID)
						}
			);

			if (listingDataModel != null && !string.IsNullOrEmpty(listingDataModel.Keyword))
				models = models.Where(e => e.Name.Contains(listingDataModel.Keyword));

			return models;
		}

		public TeamModel GetWinnerBet()
		{
			return
			(
				from	team in this.GetAll()
						join
						bet in this.Context.GlobalBets on team.TeamID equals bet.WinnerTeamID
				where	bet.UserID == this.UserID
				select	new TeamModel
                        {
							ID = team.TeamID,
							Name = team.Name,
							Flag = team.FlagPrefix,
							ExternalID = team.ExternalID,
                            Points = (int)team.Points,
							BetMade = true
						}
			).FirstOrDefault();
		}

		public override ActionStatus SaveItem(TeamModel model, DBActionType action)
		{
            //add bet only if global bets are not expired
            if (new MatchRepository().GlobalBetsExpired())
            {
                return new ActionStatus
                {
                    Success = false,
                    Message = "Bets on the winner team are not allowed anymore"
                };
            }

			return this.CallStoredProcedure
			(
				action,
				() => {	return new SPResult { Result = this.Context.SaveWinnerBet(this.UserID, model.ID) };	}
			);
		}
	}
}
