using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class StandingsRepository : Repository<User, UserModel>
	{
		public override IQueryable<IModel> GetListingItems(ListingParams<UserModel> listingDataModel)
		{
			return
			(
				from	user in new UserRepository(this.Context, this.UserID).GetActiveUsers()
				orderby	user.PointsWon descending, user.DisplayName ascending
				select	new UserModel
						{
							ID = user.UserID,
							Username = user.Username,
							DisplayName = user.DisplayName,
							Points = user.PointsWon
						}
			);
		}
	}
}
