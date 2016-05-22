using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class TeamsController : BaseApiController<WinnerRepository, Team, TeamModel>
    {
        public TeamsController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Returns teams ordered by bet points descending
        /// </summary>
        [HttpGet]
        public ActionStatusCollection<ApiTeamModel> Get(ODataQueryOptions<ApiTeamModel> queryOptions)
        {
            var result = ProjectAndFilter<ApiTeamModel>(Repo.GetWinnersForListing(null), queryOptions);
            return new ActionStatusCollection<ApiTeamModel>(result);
        }
    }
}
