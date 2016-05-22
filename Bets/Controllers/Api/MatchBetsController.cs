using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;
using Microsoft.AspNet.Identity;

namespace Bets.Controllers.Api
{
    public class MatchBetsController : BaseApiController
    {
        public MatchBetsController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Saves bet for match
        /// </summary>
        [HttpPost]
        public ActionStatus Post([FromBody]ApiMatchBetModel model)
        {
            var repo = new MatchesForRoundRepository(UserID);
            var matchForRoundModel = _mapper.Map<MatchForRoundModel>(model);

            return repo.SaveItem(matchForRoundModel, DBActionType.Insert);
        }
    }
}
