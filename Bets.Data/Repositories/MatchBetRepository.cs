using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using System.Web.Mvc;

namespace Bets.Data
{
	public class MatchBetRepository : Repository<MatchBet, MatchBetModel>
	{
		public override IQueryable<IModel> GetListingItems(ListingParams<MatchBetModel> listingDataModel)
		{
			var models =
			(
				from	matchBet in this.GetAll()
						join
						match in this.Context.Matches on matchBet.MatchID equals match.MatchID
						join
						user in new UserRepository(this.Context, this.UserID).GetActiveUsers() on matchBet.UserID equals user.UserID
				where	matchBet.MatchID == listingDataModel.Model.MatchID //bets for this match
						&& 
						matchBet.UserID != this.UserID //do not return bet made by current user
						&&
						DateTime.UtcNow >= match.Date.AddHours(-1 * DataConfig.HoursBeforeBet)  //only expired matches
				orderby	matchBet.WinTypeID descending, matchBet.PointsWonBonus descending, matchBet.PointsWon descending, user.PointsWonBonus descending, user.DisplayName ascending
				select	new MatchBetModel
						{
							ID = matchBet.BetID,
							UserDisplayName = user.DisplayName,
							Username = user.Username,
							FirstTeamGoals = matchBet.FirstTeamGoals,
							SecondTeamGoals = matchBet.SecondTeamGoals,
							Result = matchBet.Result,
							PointsWon = matchBet.PointsWon,
							PointsWonBonus = matchBet.PointsWonBonus,
							Bonus = matchBet.Bonus,
							WinTypeID = matchBet.WinTypeID ?? 0
						}
			);

			return models;
		}
	}
}
