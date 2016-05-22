using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bets.Data
{
    public class ApiSettingModel
    {
        /// <summary>
        /// Date of the first match in the tournament (UTC)
        /// </summary>
        [Required]
        public DateTime FirstMatchDate { get; set; }

        /// <summary>
        /// ID of the current round
        /// </summary>
        [Required]
        public int CurrentRoundID { get; set; }

        /// <summary>
        /// MAx number of bonus points allowed per match
        /// </summary>
        [Required]
        public int MaxBonusPerMatch { get; set; }

        /// <summary>
        /// Until how many hours before the start time of a match can a bet be made
        /// </summary>
        [Required]
        public int MatchBetCutoffHours => DataConfig.HoursBeforeBet;

        /// <summary>
        /// Until what date (UTC) can a global bet be made (goalscorer, team)
        /// </summary>
        [Required]
        public DateTime GlobalBetsExpirationDate => Extensions.GetBetCutoffDate(FirstMatchDate);
    }
}
