using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class GoalscorerStandingsRepository : Repository<Player, GoalscorerModel>
	{
		public override IQueryable<IModel> GetListingItems(ListingParams<GoalscorerModel> listingDataModel)
		{
			var models = (IQueryable<GoalscorerModel>)new GoalscorerRepository(this.Context, this.UserID).GetGoalscorersForListing(listingDataModel);

			return
			(
				from	goalscorer in models
				orderby goalscorer.GoalsScored descending, goalscorer.Points ascending, goalscorer.Name ascending
				select	goalscorer
			);
		}

		public override ActionStatus SaveItem(GoalscorerModel model, DBActionType action)
		{
			return new GoalscorerRepository(this.Context, this.UserID).SaveItem(model, action);
		}
	}
}
