using System.Web.Http;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;

namespace Bets.Controllers.Api
{
    public class SettingsController : BaseApiController<SettingsRepository, Setting, SettingModel>
    {
        public SettingsController(IMapper mapper) : base(mapper) {}

        /// <summary>
        /// Returns tournament settings
        /// </summary>
        [HttpGet]
        public ActionStatus<ApiSettingModel> Get()
        {
            var result = _mapper.Map<ApiSettingModel>(Repo.GetItem(0));
            return new ActionStatus<ApiSettingModel>(result);
        }
    }
}
