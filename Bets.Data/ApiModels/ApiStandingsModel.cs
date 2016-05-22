using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Bets.Data
{
    public class ApiStandingsModel
    {
        /// <summary>
        /// ID of user
        /// </summary>
        [Required]
        public int UserID {get; set; }

        [Required]
        public string Username {get; set; }

        [Required]
        public string DisplayName {get; set; }

        /// <summary>
        /// Points won by user
        /// </summary>
        [Required]
		public int Points {get; set; }

        /// <summary>
        /// Profile image url
        /// </summary>
        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string UserImageUrl => UIExtensions.GetUserImage(Username);
    }
}
