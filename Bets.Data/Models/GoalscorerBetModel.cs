using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class GoalscorerBetModel : Model
	{
		public int RoundID { get; set; }
		public int GoalscorerID { get; set; }
		public int Goals { get; set; }
		public string Username { get; set; }
		public string UserDisplayName { get; set; }
		public string GoalscorerName { get; set; }
		public string GoalscorerExternalID { get; set; }
		public int PointsWon { get; set; }
	}
}
