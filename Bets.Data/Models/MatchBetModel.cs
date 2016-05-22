using Bets.Data.Models;

namespace Bets.Data
{
	public class MatchBetModel : Model
	{
		public string UserDisplayName { get; set; }
		public int Result { get; set; }
		public int? PointsWon { get; set; }
		public int? PointsWonBonus { get; set; }
		public int MatchID { get; set; }
		public string Username { get; set; }
		public int? WinTypeID { get; set; }
		public int FirstTeamGoals { get; set; }
		public int SecondTeamGoals { get; set; }
		public int Bonus { get; set; }
		public bool Expired { get; set; }

		public System.DateTime Date { get; set; }
	}
}
