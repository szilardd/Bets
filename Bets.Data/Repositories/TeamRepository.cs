using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using System.Web.Mvc;

namespace Bets.Data
{
	public class TeamRepository : Repository<Team, TeamModel>
	{
		public TeamModel GetTeam(int teamID)
		{
			return this.GetTeams().Where(team => team.ID == teamID).SingleOrDefault();
		}

		public IQueryable<Team> GetTeamEntities(bool onlyActive = false)
		{
			var entities = this.GetAll();

			if (onlyActive)
				entities = entities.Where(e => e.Active);

			return entities.OrderBy(e => e.Name);
		}

		public IQueryable<TeamModel> GetTeams(bool onlyActive = false)
		{
			return this.GetTeamEntities().Select(e => new TeamModel {
				ID = e.TeamID,
				Name = e.Name,
				Flag = e.FlagPrefix
			});
		}

		public IQueryable<SelectListItem> GetTeamList(bool onlyActive = false)
		{
			return this.GetTeamEntities(onlyActive).Select(e => new SelectListItem
			{
				Value = e.TeamID.ToString(),
				Text = e.Name
			});
		}

        public List<GetAllGroupStandingsResult> GetAllGroupStandings()
        {
            return this.Context.GetAllGroupStandings().ToList();
        }

		public ActionStatus RemoveTeam(int id)
		{
			return this.CallStoredProcedure
			(
				DBActionType.Delete,
				() => { return new SPResult { Result = this.Context.RemoveTeam(id) }; }
			);
		}

        public Team GetTeamByName(string TeamName)
        {
            var entities = this.GetAll();
            entities = entities.Where(e => e.Name == TeamName);
            return entities.FirstOrDefault();
        }
    }
}
