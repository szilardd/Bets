using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
    public class WinnerPointsRepository : Repository<Team, WinnerPointsModel>
    {
        public override IQueryable<IModel> GetListingItems(ListingParams<WinnerPointsModel> listingDataModel)
        {
            var winnerTeam = new WinnerResultRepository(this.Context).GetWinnerTeam();

            return
            (
                from    globalBet in this.Context.GlobalBets
                        join
                        team in this.Context.Teams on globalBet.WinnerTeamID equals team.TeamID
                        join
                        user in this.Context.Users on globalBet.UserID equals user.UserID
                orderby (winnerTeam.ID == globalBet.WinnerTeamID) descending, user.PointsWonBonus descending, team.Points ascending
                select  new WinnerPointsModel
                        {
                            ID = team.TeamID,
                            TeamName = team.Name,
                            TeamFlagPrefix = team.FlagPrefix,
                            TeamExternalID = team.ExternalID,
                            TeamPoints = (int)team.Points,
                            BetWon = (winnerTeam.ID == globalBet.WinnerTeamID),
                            UserDisplayName = user.DisplayName,
                            Username = user.Username
                        }
            );
        }
    }
}