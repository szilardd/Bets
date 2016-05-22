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
    }
}
