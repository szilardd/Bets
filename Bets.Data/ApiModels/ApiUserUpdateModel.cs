using System;
using System.ComponentModel.DataAnnotations;

namespace Bets.Data
{
    public class ApiUserUpdateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }

        /// <summary>
        /// User's display name
        /// </summary>
        [Required]
        public string DisplayName { get; set; }
    }
}
