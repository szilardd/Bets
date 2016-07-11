using System;

namespace Bets.Data.Models
{
    public class WinnerPointsModel : Model
    {
        public string TeamFlagPrefix { get; internal set; }
        public int TeamPoints { get; internal set; }

        public string UserImageUrl => UIExtensions.GetUserImage(Username);

        public string TeamFlagUrl => UIExtensions.GetTeamFlagImage(TeamFlagPrefix);

        public string UserDisplayName { get; internal set; }
        public string TeamName { get; internal set; }
        public string TeamExternalID { get; internal set; }
        public bool BetWon { get; internal set; }
        public string Username { get; internal set; }
    }
}
