using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bets.Data.Models
{
	public class TeamModel : Model
	{
		public string Name { get; set; }
		public string Flag { get; set; }
		public bool Active { get; set; }
		public string ExternalID { get; set; }
		public bool BetMade { get; set; }
        public int Points { get; set; }
		public bool BetExpired { get; set; }
	}
}
