using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class SettingsRepository : Repository<Setting, SettingModel>
	{
		public SettingsRepository() : base() { }
		public SettingsRepository(BetsDataContext context, int userID) : base(context, userID) {}

		public override SettingModel GetItem(int id)
		{
			return this.Context.Settings.Select(e => new SettingModel
			{
				FirstMatchDate = e.FirstMatchDate,
				CurrentRoundID = e.CurrentRoundID,
				LastNotificationRoundID = e.LastNotificationRoundID,
				MaxBonusPerMatch = e.MaxBonusPerMatch
			}).First();
		}

		public void SetLastNotificationRoundID()
		{
			this.CallStoredProcedure
			(
				DBActionType.Update, 
				() => 
				{ 
					return new SPResult { Result = this.Context.UpdateLastNotificationRoundID() };
				}
			);
		}

        public bool GetTournamentEndStatus()
        {
            return (this.Context.Teams.Count(e => e.Active) == 1);
        }
    }
}
