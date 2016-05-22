using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using Bets.Data;
using System.Web;

namespace Bets.Data.Models
{
    public class RoundMatchesForUserModel : MatchModel
    {
        public int RoundID { get; set; }
        public bool Current { get; set; }

        public int? BetFirstTeamGoals { get; set; }
        public int? BetSecondTeamGoals { get; set; }
        public int? BetResult { get; set; }
        public int? BetPointsWon { get; set; }
        public int? WinTypeID { get; set; }
        public int Bonus { get; set; }
    }
}
