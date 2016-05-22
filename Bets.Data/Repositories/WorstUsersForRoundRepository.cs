using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class WorstUsersForRoundRepository : Repository<User, UserModel>
	{
		public override IQueryable<IModel> GetListingItems(ListingParams<UserModel> listingDataModel)
		{
			return
			(
				from	user in new UserRepository(this.Context, this.UserID).GetActiveUsers()
						join
						pointForRound in this.Context.PointsForRounds on user.UserID equals pointForRound.UserID
						join
						setting in this.Context.Settings on pointForRound.RoundID equals listingDataModel.Model.RoundID
						join
						round in this.Context.Rounds on pointForRound.RoundID equals round.RoundID
				where	(this.IsAdmin || round.Closed)
				orderby	pointForRound.MatchPointsBonus + pointForRound.GoalscorerPoints ascending
				select	new UserModel
						{
							ID = user.UserID,
							Username = user.Username,
							DisplayName = user.DisplayName,
							Points = pointForRound.MatchPointsBonus + pointForRound.GoalscorerPoints
						}
			).Take(5);
		}
	}
}
