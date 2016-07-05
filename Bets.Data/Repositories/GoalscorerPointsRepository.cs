using System;
using System.Linq;
using Bets.Data.Models;

namespace Bets.Data
{
    public class GoalscorerPointsRepository : Repository<Team, GoalscorerPointsModel>
    {
        public override IQueryable<IModel> GetListingItems(ListingParams<GoalscorerPointsModel> listingDataModel)
        {
            var goalscorerEntities = new GoalscorerResultRepository(this.Context).GetTopGoalscorerEntities();

            return
            (
                from    globalBet in this.Context.GlobalBets
                        join
                        player in this.Context.Players on globalBet.GoalscorerID equals player.PlayerID
                        join
                        team in this.Context.Teams on player.TeamID equals team.TeamID
                        join
                        user in this.Context.Users on globalBet.UserID equals user.UserID
                        join
                            allTopGoalscorer in goalscorerEntities on player.PlayerID equals allTopGoalscorer.ID into allTopGoalscorers
                            from topGoalscorer in allTopGoalscorers.DefaultIfEmpty()
                orderby (topGoalscorer != null) descending, user.PointsWonBonus descending, player.Points ascending
                select  new GoalscorerPointsModel
                        {
                            ID = player.PlayerID,
                            PlayerName = player.Name,
                            PlayerTeamFlagPrefix = team.FlagPrefix,
                            PlayerExternalID = player.ExternalID,
                            PlayerPoints = (int)player.Points,
                            PlayerGoalsScored = player.GoalsScored,
                            BetWon = (topGoalscorer != null),
                            UserDisplayName = user.DisplayName,
                            Username = user.Username
                        }
            );
        }
    }
}