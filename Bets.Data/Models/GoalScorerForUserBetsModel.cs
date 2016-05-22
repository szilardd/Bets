using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using Bets.Data;
using System.Web;

namespace Bets.Data.Models
{
    public class GoalScorerForUserBetsModel : GoalscorerModel
    {
        public int? UserID { get; set; }

        public string TeamFlag { get; set; }
        public int GoalsScored { get; set; }
        public string ExternalID { get; set; }
        public bool BetMade { get; set; }
        public bool BetExpired { get; set; }

        public int Points { get; set; }
    }
}
