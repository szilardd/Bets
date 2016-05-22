using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Bets.Data
{
    public class ApiGoalscorerModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name {get; set; }

        [JsonIgnore] // ignore when serializing
		public string TeamFlag {get; set; }

        /// <summary>
        /// First team flag image url
        /// </summary>
        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string TeamFlagUrl => UIExtensions.GetTeamFlagImage(TeamFlag);

        [Required]
        public int GoalsScored {get; set; }

        [JsonIgnore]  // ignore when serializing
        public string ExternalID {get; set; }

        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string ImageUrl => UIExtensions.GetPlayerImage(ExternalID);

        /// <summary>
        /// Points to win if goalscorer becomes the top goalscorer of the tournament
        /// </summary>
        [Required]
        public int Points {get; set; }

        /// <summary>
        /// Whether a bet was made on this player
        /// </summary>
        [Required]
        public bool BetMade { get; set; }
    }
}
