using System;
using System.Linq;
using Bets.Data.Models;

namespace Bets.Data
{
    public class WinnerResultRepository : Repository<Team, TeamModel>
    {
        public WinnerResultRepository() :base()
        {
        }

        public WinnerResultRepository(BetsDataContext context) : base(context)
        {
        }

        public override IQueryable<IModel> GetListingItems(ListingParams<TeamModel> listingDataModel)
        {
            return GetWinnerTeamEntities();
        }

        public IQueryable<TeamModel> GetWinnerTeamEntities()
        {
            return
            (
                from    team in this.GetAll()
                where   team.Active
                select  new TeamModel
                        {
                            ID = team.TeamID,
                            Name = team.Name,
                            Flag = team.FlagPrefix,
                            ExternalID = team.ExternalID,
                            Points = (int)team.Points
                        }
           );
        }

        public TeamModel GetWinnerTeam()
        {
            return GetWinnerTeamEntities().FirstOrDefault();
        }
    }
}