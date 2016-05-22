using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Web.OData;
using AutoMapper;

using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class StandingsController : BaseApiController
    {
        private readonly StandingsRepository _repo;

        public StandingsController(IMapper mapper) : base(mapper)
        {
            _repo = new StandingsRepository();
        }

        /// <summary>
        /// Returns user standings in descending order by number of points won
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public ActionStatusCollection<ApiStandingsModel> Get(ODataQueryOptions<ApiStandingsModel> queryOptions)
        {
            return new ActionStatusCollection<ApiStandingsModel>(ProjectAndFilter(_repo.GetListingItems(null), queryOptions));
        }
    }
}
