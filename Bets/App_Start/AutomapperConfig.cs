using System;
using AutoMapper;
using Bets.Data;
using Bets.Data.Models;

namespace Bets
{
    public class AutomapperConfig
    {
        public static IMapper Register()
        {
            var config = new MapperConfiguration
            (
                cfg => {
                    
                    // in
                    cfg.CreateMap<ApiMatchBetModel, MatchForRoundModel>().ForMember(e => e.ID, m => m.MapFrom(a => a.MatchID));
                    cfg.CreateMap<ApiGoalScorerBetModel, GoalscorerModel>();
                    cfg.CreateMap<ApiTeamBetModel, TeamModel>();
                    cfg.CreateMap<ApiUserUpdateModel, UserModel>();

                    // out
                    cfg.CreateMap<MatchForRoundModel, ApiMatchForRoundModel>().ForMember(e => e.MatchID, m => m.MapFrom(a => a.ID));
                    cfg.CreateMap<UserModel, ApiStandingsModel>().ForMember(e => e.UserID, m => m.MapFrom(a => a.ID));
                    cfg.CreateMap<GoalscorerModel, ApiGoalscorerModel>();
                    cfg.CreateMap<TeamModel, ApiTeamModel>();
                    cfg.CreateMap<SettingModel, ApiSettingModel>();
                    cfg.CreateMap<UserModel, ApiUserModel>();
                }
            );

            return config.CreateMapper();
        }
    }
}
