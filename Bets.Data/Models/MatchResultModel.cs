using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bets.Data.Models
{
    public class MatchResultModel
    {
        public string MinuteOrResult { get; set; }
        public string FirstTeamName { get; set; }
        public string SecondTeamName { get; set; }

        public int FirstTeamGoals { get; set; }
        public int SecondTeamGoals { get; set; }
    }
}
