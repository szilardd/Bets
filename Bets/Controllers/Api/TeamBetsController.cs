using System.Linq;
using System.Web.Http;
using AutoMapper;

using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class TeamBetsController : BaseApiController
    {
        private readonly WinnerRepository _repo;

        public TeamBetsController(IMapper mapper) : base(mapper)
        {
            _repo = new WinnerRepository(UserID);
        }

        /// <summary>
        /// Returns team bet
        /// </summary>
        [HttpGet]
        public ActionStatus<ApiTeamModel> Get(int id)
        {
            var result = _mapper.Map<ApiTeamModel>(_repo.GetSelectedWinner(null).FirstOrDefault());
            return new ActionStatus<ApiTeamModel>(result);
        }

        /// <summary>
        /// Saves team bet
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionStatus Post(ApiTeamBetModel model)
        {
            return _repo.SaveItem(_mapper.Map<TeamModel>(model), DBActionType.Update);
        }
    }
}
