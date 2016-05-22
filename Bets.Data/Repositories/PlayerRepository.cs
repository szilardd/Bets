using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;

namespace Bets.Data
{
	public class PlayerRepository : Repository<Player, PlayerModel>
	{
		public PlayerRepository() : base() { }
		public PlayerRepository(BetsDataContext context, int userID) : base(context, userID) {}

		public IQueryable<Player> GetActivePlayers()
		{
			return this.GetAll().Where(e => e.Active);
		}

		public IEnumerable<DropdownOption> GetPlayerGoalsForCurrentRound()
		{
			return
			(
				from	player in this.GetActivePlayers()
				from	setting in this.Context.Settings
						join
							allGoalscorerForRound in this.Context.GoalscorerForRounds on 
							new { PlayerID = player.PlayerID, RoundID = setting.CurrentRoundID } equals 
							new { PlayerID = allGoalscorerForRound.GoalscorerID, RoundID = allGoalscorerForRound.RoundID } into allGoalscorersForRound
							from goalscorerForRound in allGoalscorersForRound.DefaultIfEmpty()
				select	new DropdownOption
						{
							Value = player.PlayerID.ToString(),
							Text = player.Name,
							CustomData = goalscorerForRound.Goals.ToString(),
							CustomData2 = player.GoalsScored.ToString()
						}
			).ToList();
		}

		public ActionStatus AddGoalscorerForRound(GoalscorerBetModel model)
		{
			return this.CallStoredProcedure
			(
				DBActionType.Insert,
				() =>
				{
					return new SPResult { Result = this.Context.SaveGoalscorerForCurrentRound(model.GoalscorerID, model.Goals) };
				}
			);
		}

		public ActionStatus RemovePlayer(int id)
		{
			return this.CallStoredProcedure
			(
				DBActionType.Delete,
				() => { return new SPResult { Result = this.Context.RemovePlayer(id) }; }
			);
		}
	}
}
