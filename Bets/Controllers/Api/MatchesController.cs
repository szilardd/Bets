using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData.Query;
using AutoMapper;
using Bets.Data;

namespace Bets.Controllers.Api
{
    public class MatchesController : BaseApiController
    {
        private readonly MatchesForRoundRepository _repo;

        public MatchesController(IMapper mapper) : base(mapper)
        {
            _repo = new MatchesForRoundRepository(UserID);
        }

        /// <summary>
        /// Returns matches for the current round
        /// </summary>
        /// <returns>List of matches</returns>
        [HttpGet]
        public ActionStatusCollection<ApiMatchForRoundModel> Get(ODataQueryOptions<ApiMatchForRoundModel> queryOptions)
        {
            var result = ProjectAndFilter(_repo.GetMatchesForRound(null), queryOptions);
            return new ActionStatusCollection<ApiMatchForRoundModel>(result);
        }
    }
}
