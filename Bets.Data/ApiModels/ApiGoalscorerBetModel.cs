using System;
using System.ComponentModel.DataAnnotations;

namespace Bets
{
    public class ApiGoalScorerBetModel
    {
        /// <summary>
        /// ID of the goalscorer
        /// </summary>
        [Required]
        public int ID { get; set; }
    }
}
