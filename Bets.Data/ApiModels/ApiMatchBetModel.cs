using System;
using System.ComponentModel.DataAnnotations;

namespace Bets.Data
{
    public class ApiMatchBetModel
    {
        [Required]
        public int MatchID { get; set; }

        [Required]
        public int? FirstTeamGoals { get; set; }

        [Required]
        public int? SecondTeamGoals { get; set; }

        [Required]
        public int? Bonus { get; set; }
    }
}