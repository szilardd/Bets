using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bets.Data
{
    public class ApiUserModel : ApiUserUpdateModel
    {
        /// <summary>
        /// Profile image url
        /// </summary>
        [NotMapped] // ignore in OData query but serialize
        [Required]
        public string ImageUrl => UIExtensions.GetUserImage(Username);

        /// <summary>
        /// Number of bonus points available for user to bet
        /// </summary>
        [Required]
        public int Bonus { get; internal set; }
    }
}
