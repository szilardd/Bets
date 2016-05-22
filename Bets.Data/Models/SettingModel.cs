using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bets.Data.Models
{
	public class SettingModel : Model
	{
		public DateTime FirstMatchDate { get; set; }
		public int CurrentRoundID { get; set; }
		public int? LastNotificationRoundID { get; set; }
		public int MaxBonusPerMatch { get; set; }
	}
}
