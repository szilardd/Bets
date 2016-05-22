using System.Web.Http;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class UsersController : BaseApiController<UserRepository, User, UserModel>
    {
        public UsersController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Returns logged in user's details
        /// </summary>
        [HttpGet]
        public ActionStatus<ApiUserModel> Get()
        {
            var result = _mapper.Map<ApiUserModel>(Repo.GetUserByID());
            return new ActionStatus<ApiUserModel>(result);
        }

        /// <summary>
        /// Saves user details
        /// </summary>
        [HttpPost]
        public ActionStatus Post(ApiUserUpdateModel model)
        {
            return Repo.SaveItem(_mapper.Map<UserModel>(model), DBActionType.Update);
        }
    }
}
