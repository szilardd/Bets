using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class MatchesController : BaseApiController<MatchesForRoundRepository, Match, MatchForRoundModel>
    {
        public MatchesController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Returns matches for the current round
        /// </summary>
        /// <returns>List of matches</returns>
        [HttpGet]
        public ActionStatusCollection<ApiMatchForRoundModel> Get(ODataQueryOptions<ApiMatchForRoundModel> queryOptions)
        {
            var result = ProjectAndFilter(Repo.GetMatchesForRound(null), queryOptions);
            return new ActionStatusCollection<ApiMatchForRoundModel>(result);
        }
    }
}
