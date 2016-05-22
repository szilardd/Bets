using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Bets.Data
{
    public class ApiTeamModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        
        [JsonIgnore] // ignore in OData query but serialize
        public string ExternalID { get; set; }

        [JsonIgnore]
        public string Flag { get; set; }

        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string ImageUrl => UIExtensions.GetTeamLogoImage(ExternalID);

        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string FlagImageUrl => UIExtensions.GetTeamFlagImage(Flag);

        /// <summary>
        /// Whether user has made a bet on this team
        /// </summary>
        [Required]
        public bool BetMade { get; set; }

        /// <summary>
        /// Points to win if teams wins the tournament
        /// </summary>
        [Required]
        public int Points { get; set; }
    }
}
