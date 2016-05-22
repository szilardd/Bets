using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;
using System.Web.OData;
using AutoMapper;

using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class GoalscorersController : BaseApiController<GoalscorerRepository, Player, GoalscorerModel>
    {
        private readonly GoalscorerRepository _repo;

        public GoalscorersController(IMapper mapper) : base(mapper)
        {
            _repo = new GoalscorerRepository(UserID);
        }

        /// <summary>
        /// Returns goalscorer ordered by number of goals scored descending then by number of bet points ascending
        /// </summary>
        /// <returns>List of goalscorers</returns>
        [HttpGet]
        public ActionStatusCollection<ApiGoalscorerModel> Get(ODataQueryOptions<ApiGoalscorerModel> queryOptions)
        {
            var result = ProjectAndFilter(_repo.GetGoalscorersForListing(null), queryOptions);
            return new ActionStatusCollection<ApiGoalscorerModel>(result);
        }
    }
}
