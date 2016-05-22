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
    public class MatchBetsController : BaseApiController<MatchesForRoundRepository, Match, MatchForRoundModel>
    {
        public MatchBetsController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Saves bet for match
        /// </summary>
        [HttpPost]
        public ApiMatchBetResponse Post([FromBody]ApiMatchBetModel model)
        {
            var matchForRoundModel = _mapper.Map<MatchForRoundModel>(model);

            var saveResult = Repo.SaveItem(matchForRoundModel, DBActionType.Update);
            ApiMatchBetResponse response = new ApiMatchBetResponse(saveResult);

            // set user bonus
            if (saveResult.Success)
            {
                response.Result = Repo.GetUserBonus();
            }

            return response;
        }
    }
}
