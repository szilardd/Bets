using System.ComponentModel.DataAnnotations;

namespace Bets
{
    public class ApiTeamBetModel
    {
        /// <summary>
        /// ID of the team
        /// </summary>
        [Required]
        public int ID { get; set; }
    }
}