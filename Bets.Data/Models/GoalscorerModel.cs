using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bets.Data.Models
{
	public class GoalscorerModel : PlayerModel
	{
		public string TeamFlag { get; set; }
		public int GoalsScored { get; set; }
		public string ExternalID { get; set; }
		public bool BetMade { get; set; }
		public bool BetExpired { get; set; }
		public int Points { get; set; }
		public bool OnlySelected { get; set; }
        public string ShortName => Extensions.GetShortName(Name);
    }
}
