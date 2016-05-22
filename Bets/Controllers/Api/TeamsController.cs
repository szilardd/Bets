using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using AutoMapper;
using Bets.Data;

namespace Bets.Controllers.Api
{
    public class TeamsController : BaseApiController
    {
        private readonly WinnerRepository _repo;

        public TeamsController(IMapper mapper) : base(mapper)
        {
            _repo = new WinnerRepository(UserID);
        }

        /// <summary>
        /// Returns teams ordered by bet points descending
        /// </summary>
        [HttpGet]
        public ActionStatusCollection<ApiTeamModel> Get(ODataQueryOptions<ApiTeamModel> queryOptions)
        {
            var result = ProjectAndFilter<ApiTeamModel>(_repo.GetWinnersForListing(null), queryOptions);
            return new ActionStatusCollection<ApiTeamModel>(result);
        }
    }
}
