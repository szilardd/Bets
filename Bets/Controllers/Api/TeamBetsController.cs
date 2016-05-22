using System.Linq;
using System.Web.Http;
using AutoMapper;

using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class TeamBetsController : BaseApiController<WinnerRepository, Team, TeamModel>
    {
        public TeamBetsController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Returns team bet
        /// </summary>
        [HttpGet]
        public ActionStatus<ApiTeamModel> Get()
        {
            var result = _mapper.Map<ApiTeamModel>(Repo.GetSelectedWinner(null).FirstOrDefault());
            return new ActionStatus<ApiTeamModel>(result);
        }

        /// <summary>
        /// Saves team bet
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionStatus Post(ApiTeamBetModel model)
        {
            return Repo.SaveItem(_mapper.Map<TeamModel>(model), DBActionType.Update);
        }
    }
}
