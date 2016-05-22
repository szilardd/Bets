using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Web.OData;
using AutoMapper;

using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class GoalscorerBetsController : BaseApiController<GoalscorerRepository, Player, GoalscorerModel>
    {
        public GoalscorerBetsController(IMapper mapper) : base(mapper)
        {
        }

        /// <summary>
        /// Returns goalscorer bet
        /// </summary>
        [HttpGet]
        public ActionStatus<ApiGoalscorerModel> Get()
        {
            var result = _mapper.Map<ApiGoalscorerModel>(Repo.GetSelectedGoalscorer(null).FirstOrDefault());
            return new ActionStatus<ApiGoalscorerModel>(result);
        }

        /// <summary>
        /// Saves goalscorer bet
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionStatus Post(ApiGoalScorerBetModel model)
        {
            var repo = new GoalscorerRepository(UserID);
            return repo.SaveItem(_mapper.Map<GoalscorerModel>(model), DBActionType.Update);
        }
    }
}
