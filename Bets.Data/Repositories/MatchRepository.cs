using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using Bets.Data;

namespace Bets.Data
{
	public class MatchRepository : Repository<Match, MatchModel>
	{
		public MatchRepository() : base() { }
		public MatchRepository(BetsDataContext context) : base(context) { }
		public MatchRepository(BetsDataContext context, int userID) : base(context, userID) { }

		public bool AddMatches(List<MatchModel> matches)
		{
			if (matches == null)
				return true;

			foreach (var match in matches)
			{
				this.Add(new Match 
				{
					FirstTeamID = match.FirstTeamID,
					SecondTeamID = match.SecondTeamID,
					Date = match.Date.FromEETToUTC(),
					Points1 = match.Points1,
					PointsX = match.PointsX,
					Points2 = match.Points2,
					RoundID = match.RoundID
				});
			}

			return this.SaveContext();
		}

		public IQueryable<MatchModel> GetMatchesForCurrentRound()
		{
			return
			(
				from	match in this.Context.Matches
						join
						firstTeam in this.Context.Teams on match.FirstTeamID equals firstTeam.TeamID
						join
						secondTeam in this.Context.Teams on match.SecondTeamID equals secondTeam.TeamID
				select	new MatchModel
						{
							ID = match.MatchID,
							FirstTeamName = firstTeam.Name,
							SecondTeamName = secondTeam.Name,
							Date = match.Date
						}
			);
		}

		/// <summary>
		/// Returns true if betting period for current round has expired (date of first match in UTC!)
		/// </summary>
		/// <returns></returns>
		public bool CurrentRoundIsExpired()
		{
			object firstMatchDate =
			(
				from	match in this.GetAll()
						join
						settings in this.Context.Settings on match.RoundID equals settings.CurrentRoundID
				select	(DateTime?)match.Date
			).Min();

			return firstMatchDate == null || Convert.ToDateTime(firstMatchDate) < Extensions.GetExpirationDate();
		}

		/// <summary>
		/// Returns true if betting period for global bets has expired (date of the competition's first match in UTC!)
		/// </summary>
		/// <returns></returns>
		public bool GlobalBetsExpired()
		{
			return this.Context.Settings.Single().FirstMatchDate < Extensions.GetExpirationDate();
		}

		public IEnumerable<DropdownOption> GetActiveMatchList()
		{
			return
			(
				from	match in this.GetAll()
				from	setting in this.Context.Settings
						join
						firstTeam in this.Context.Teams on match.FirstTeamID equals firstTeam.TeamID
						join
						secondTeam in this.Context.Teams on match.SecondTeamID equals secondTeam.TeamID
				where	setting.CurrentRoundID == match.RoundID || match.FirstTeamGoals == null
				select	new DropdownOption
						{
							Value = match.MatchID.ToString(),
							Text = firstTeam.Name + " - " + secondTeam.Name
						}
			).ToList();
		}

		public ActionStatus AddMatchResult(MatchModel match)
		{
			return this.CallStoredProcedure
			(
				DBActionType.Insert,
				() => {
					return new SPResult { Result = this.Context.SaveMatchResult(match.ID, match.FirstTeamGoals, match.SecondTeamGoals) };
				}
			);
		}
	}
}
