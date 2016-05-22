using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using Newtonsoft.Json;

namespace Bets.Data.Models
{
	public class MatchModel : Model
	{
		public int FirstTeamID { get; set; }
		public int SecondTeamID { get; set; }

        public DateTime Date { get; set; }

		public string FirstTeamName { get; set; }
		public string SecondTeamName { get; set; }

		public string FirstTeamFlag { get; set; }
		public string SecondTeamFlag { get; set; }

		public int? FirstTeamGoals { get; set; }
		public int? SecondTeamGoals { get; set; }
		public int? Result { get; set; }

		public bool Expired { get; set; }

		public int Points1 { get; set; }
		public int PointsX { get; set; }
		public int Points2 { get; set; }
		public int RoundID { get; set; }
	}
}
