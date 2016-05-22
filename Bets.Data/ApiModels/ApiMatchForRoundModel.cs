using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Bets.Data
{
    public class ApiMatchForRoundModel
    {
        [Required]
        public int MatchID {get; set; }

        [Required]
        public int FirstTeamID { get; set; }

        [Required]
        public string FirstTeamName {get; set; }

        [Required]
        public int SecondTeamID { get; set; }

        [Required]
        public string SecondTeamName {get; set; }

        /// <summary>
        /// Round in which the match takes place
        /// </summary>
        [Required]
        public int RoundID { get; set; }

        /// <summary>
        /// Until what date (UTC) can a bet be made on the match
        /// </summary>
        [Required]
        [NotMapped]
        public DateTime BetExpirationDate => Extensions.GetBetCutoffDate(Date);

        /// <summary>
        /// Points awared if the first team wins
        /// </summary>
        [Required]
		public int Points1 {get; set; }

        /// <summary>
        /// Points awarded on draw
        /// </summary>
        [Required]
        public int PointsX {get; set; }

        /// <summary>
        /// Points awarded if the second team wins
        /// </summary>
        [Required]
        public int Points2 {get; set; }

        /// <summary>
        /// Match date in UTC format
        /// </summary>
        [Required]
        public DateTime Date {get; set; }

        /// <summary>
        /// First team flag image name (e.g. ger.png). Use flag base url from setting to determine the full url
        /// </summary>
        [JsonIgnore]
        public string FirstTeamFlag {get; set; }

        /// <summary>
        /// Second team flag image name (e.g. bra.png). Use flag base url from setting to determine the full url
        /// </summary>
        [JsonIgnore]
        public string SecondTeamFlag {get; set; }

        /// <summary>
        /// First team flag image url
        /// </summary>
        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string FirstTeamFlagUrl => UIExtensions.GetTeamFlagImage(FirstTeamFlag);

        /// <summary>
        /// Second team flag image url
        /// </summary>
        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string SecondTeamFlagUrl => UIExtensions.GetTeamFlagImage(SecondTeamFlag);

        /// <summary>
        /// Goals scored by the first team
        /// </summary>
		public int? FirstTeamGoals {get; set; }

        /// <summary>
        /// Goals scored by the second team
        /// </summary>
		public int? SecondTeamGoals {get; set; }

        /// <summary>
        /// Match result.
        /// 0 = first team won,
        /// 1 = draw,
        /// 2 = second team won
        /// </summary>
		public int? Result {get; set; }

        /// <summary>
        /// Bet for the goals scored by the first team
        /// </summary>
		public int? BetFirstTeamGoals {get; set; }

        /// <summary>
        /// Bet for the goals scored by the second team
        /// </summary>
		public int? BetSecondTeamGoals {get; set; }

        /// <summary>
        /// Bet for match outcome.
        /// 0 = first team wins,
        /// 1 = draw,
        /// 2 = second team wins
        /// </summary>
		public int? BetResult {get; set; }

        /// <summary>
        /// Points won without bonus points
        /// </summary>
		public int? BetPointsWon {get; set; }

        /// <summary>
        /// Points won with bonus points
        /// </summary>
		public int? BetPointsWonBonus {get; set; }

        /// <summary>
        /// Bonus points added to the bet
        /// </summary>
		public int? Bonus {get; set; }

        /// <summary>
        /// Whether the bet for the match has expired
        /// </summary>
        [Required]
        public bool Expired {get; set; }

        /// <summary>
        /// Win type.
        /// 0 = match hasn't finished yet
		/// 1 = nothing,
		/// 2 = match outcome,
		/// 3 = exact score
        /// </summary>
        [Required]
        public int WinTypeID {get; set; }
    }
}
