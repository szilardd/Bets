using System;

namespace Bets.Data.Models
{
    public class GoalscorerPointsModel : Model
    {
        public bool BetWon { get; set; }
        public string PlayerExternalID { get; set; }
        public string PlayerTeamFlagPrefix { get; set; }
        public string PlayerName { get; set; }
        public string PlayerShortName => Extensions.GetShortName(PlayerName);
        public int PlayerPoints { get; set; }
        public int PlayerGoalsScored { get; set; }
        public string UserDisplayName { get; set; }
        public string Username { get; set; }

        public string UserImageUrl => UIExtensions.GetUserImage(Username);

        public string PlayerTeamFlagUrl => UIExtensions.GetTeamFlagImage(PlayerTeamFlagPrefix);
    }
}