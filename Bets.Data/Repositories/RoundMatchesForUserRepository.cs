using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class RoundMatchesForUserRepository : Repository<Match, MatchForRoundModel>
    {
		public override IQueryable<IModel> GetListingItems(ListingParams<MatchForRoundModel> listingDataModel)
		{
			return new MatchesForRoundRepository(this).GetListingItems(listingDataModel);
		}

        public IQueryable<Match> GetRoundBetsForUser()
        {
            return this.GetAll().OrderByDescending(e => e.RoundID);
        }
    }
}
