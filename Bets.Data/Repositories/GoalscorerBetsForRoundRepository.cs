using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class GoalscorerBetsForRoundRepository : Repository<Player, GoalscorerBetModel>
	{
		/// <summary>
		/// Retrieves goalscorer bets for all users for a single round
		/// </summary>
		public override IQueryable<IModel> GetListingItems(ListingParams<GoalscorerBetModel> listingDataModel)
		{
			return
			(
				from	betForRound in this.Context.BetsForRounds
						join
						user in new UserRepository(this.Context, this.UserID).GetActiveUsers() on betForRound.UserID equals user.UserID
						join
						goalscorer in this.Context.Players on betForRound.GoalscorerID equals goalscorer.PlayerID
						join
						pointForRound in this.Context.PointsForRounds on 
						new { UserID = betForRound.UserID, RoundID = betForRound.RoundID } equals new { UserID = pointForRound.UserID, RoundID = pointForRound.RoundID }
						join
						round in this.Context.Rounds on pointForRound.RoundID equals round.RoundID
				where	(this.IsAdmin || round.Closed) && betForRound.RoundID == listingDataModel.Model.RoundID
				orderby pointForRound.GoalscorerPoints descending, user.Username
				select	new GoalscorerBetModel
						{
							ID = betForRound.BetID,
							Username = user.Username,
							UserDisplayName = user.DisplayName,
							GoalscorerName = goalscorer.Name,
							GoalscorerExternalID = goalscorer.ExternalID,
							PointsWon = pointForRound.GoalscorerPoints
						}
			);
		}
	}
}
