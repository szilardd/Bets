using System;
using System.Linq;
using Bets.Data.Models;

namespace Bets.Data
{
    public class GoalscorerResultRepository : Repository<Player, GoalscorerModel>
    {
        public GoalscorerResultRepository() : base() 
        {

        }

        public GoalscorerResultRepository(BetsDataContext context) : base(context)
        {
        }

        public override IQueryable<IModel> GetListingItems(ListingParams<GoalscorerModel> listingDataModel)
        {
            return this.GetTopGoalscorerEntities();
        }

        public IQueryable<GoalscorerModel> GetTopGoalscorerEntities()
        {
             var maxGoals = this.Context.Players.Select(e => e.GoalsScored).Max();

            return
            (
                from    goalscorer in this.GetAll()
                        join
                        team in this.Context.Teams on goalscorer.TeamID equals team.TeamID
                where   goalscorer.GoalsScored == maxGoals
                select  new GoalscorerModel
                        {
                            ID = goalscorer.PlayerID,
                            Name = goalscorer.Name,
                            TeamFlag = team.FlagPrefix,
                            GoalsScored = goalscorer.GoalsScored,
                            ExternalID = goalscorer.ExternalID,
                            Points = goalscorer.Points
                        }
            );
        }
    }
}