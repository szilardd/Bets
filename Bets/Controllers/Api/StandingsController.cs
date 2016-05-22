using System;
using System.Web.Http;
using System.Web.Http.OData.Query;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class StandingsController : BaseApiController<StandingsRepository, User, UserModel>
    {
        public StandingsController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Returns user standings in descending order by number of points won
        /// </summary>
        [HttpGet]
        public ActionStatusCollection<ApiStandingsModel> Get(ODataQueryOptions<ApiStandingsModel> queryOptions)
        {
            var result = ProjectAndFilter(Repo.GetListingItems(null), queryOptions);
            return new ActionStatusCollection<ApiStandingsModel>(result);
        }
    }
}
