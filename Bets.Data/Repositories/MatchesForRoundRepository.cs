using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using System.Configuration;

namespace Bets.Data
{
	public enum WinType
	{
		None = 0,
		Nothing = 1,
		Result = 2,
		Score = 3
	}

	public class MatchesForRoundRepository : Repository<Match, MatchForRoundModel>
	{
		public MatchesForRoundRepository() : base() { }
		public MatchesForRoundRepository(Repository<Match, MatchForRoundModel> repo) : base(repo) { }
        public MatchesForRoundRepository(int userID) : base(userID) { }
        public MatchesForRoundRepository(BetsDataContext context, int userID) : base(context, userID) { }
        public MatchesForRoundRepository(BetsDataContext context) : base(context) { }

        /// <summary>
        /// Is the match expired ?
        /// </summary>
        public static Func<DateTime, bool> IsExpired = matchDate => (matchDate == null) ? false : (matchDate <= Extensions.GetExpirationDate());

		public override IQueryable<IModel> GetListingItems(ListingParams<MatchForRoundModel> listingDataModel)
		{
			return this.GetMatchesForRound(listingDataModel);
		}

		public IQueryable<MatchForRoundModel> GetMatchesForRound(ListingParams<MatchForRoundModel> listingDataModel = null)
		{
			//if the round is not the current one, retrive matches from the next round
			int offset = (listingDataModel != null && listingDataModel.Model.Current) ? 0 : 1;
			int userID = this.UserID;

			var entities = this.GetAll();

			if (listingDataModel != null)
			{
				//if round ID is not set, return only matches in the current round
				if (listingDataModel.Model.RoundID == 0)
				{
					entities =
					(
						from	match in entities
								join
								setting in this.Context.Settings on match.RoundID equals setting.CurrentRoundID + offset
						select	match
					);
				}
				//otherwise, return only matches in the given round, but only if the round is finished or permission is granted
				else
				{
					entities = 
					(
						from	match in entities
								join
								round in this.Context.Rounds on match.RoundID equals round.RoundID
						where	(this.IsAdmin || round.Finished || listingDataModel.Model.ForNotification) && match.RoundID == listingDataModel.Model.RoundID
						select	match
					);
				}
				//if user id is received, default to that, otherwise the query will run for the logged in user
				if (listingDataModel.Model.UserID != null)
					userID = listingDataModel.Model.UserID.Value;
			}
			
			var models = 
			(
				from	match in entities
						join
						firstTeam in this.Context.Teams on match.FirstTeamID equals firstTeam.TeamID
						join
						secondTeam in this.Context.Teams on match.SecondTeamID equals secondTeam.TeamID
						join
							allBet in this.Context.MatchBets on new { MatchID = match.MatchID, UserID = userID } equals new { MatchID = allBet.MatchID, UserID = allBet.UserID } into allBets
							from bet in allBets.DefaultIfEmpty()
				orderby match.Date
				select	new MatchForRoundModel
						{
							ID = match.MatchID,
                            FirstTeamID = firstTeam.TeamID,
							FirstTeamName = firstTeam.Name,
                            SecondTeamID = secondTeam.TeamID,
							SecondTeamName = secondTeam.Name,
							Points1 = match.Points1,
							PointsX = match.PointsX,
							Points2 = match.Points2,
							Date = match.Date,
                            RoundID = match.RoundID,
							FirstTeamFlag = firstTeam.FlagPrefix,
							SecondTeamFlag = secondTeam.FlagPrefix,
							FirstTeamGoals = match.FirstTeamGoals,
							SecondTeamGoals = match.SecondTeamGoals,
							Result = match.Result,
							BetFirstTeamGoals = bet.FirstTeamGoals,
							BetSecondTeamGoals = bet.SecondTeamGoals,
							BetResult = bet.Result,
							BetPointsWon = bet.PointsWon,
							BetPointsWonBonus = bet.PointsWonBonus,
							Bonus = bet.Bonus,
							Expired = IsExpired(match.Date),
							WinTypeID = bet.WinTypeID ?? 0
						}
			);

			if (listingDataModel != null && !String.IsNullOrEmpty(listingDataModel.Keyword))
				models = models.Where(e => e.FirstTeamName.Contains(listingDataModel.Keyword) || e.SecondTeamName.Contains(listingDataModel.Keyword));

			return models;
		}

		public override ActionStatus SaveItem(MatchForRoundModel model, DBActionType action)
		{
			var date = this.Context.Matches.Where(e => e.MatchID == model.ID).Select(e => e.Date).FirstOrDefault();

            //add bet only if round is not expired
            if (IsExpired(date))
            {
                return new ActionStatus
                {
                    Success = false,
                    Message = "Bets for this match have expired",
                    Result = Extensions.GetBetCutoffDate(date)
                };
            }

			return this.CallStoredProcedure
			(
				action,
				() =>
				{
					return new SPResult { Result = this.Context.SaveMatchBet(model.ID, this.UserID, model.FirstTeamGoals, model.SecondTeamGoals, model.Bonus, DataConfig.HoursBeforeBet) };
				}
			);
		}

		public int GetUserBonus()
		{
			return this.Context.Users.Where(e => e.UserID == this.UserID).First().Bonus;
		}

		/// <summary>
		/// Retrieves today's matches on which there are no bets made by users and groups matches by user
		/// </summary>
		public Dictionary<string, List<MatchWithNoBetModel>> GetMatchesWithNoBetsForToday()
		{
			var today = DateTime.UtcNow.Date;

			//retrieve matches with no bets
			var matches =
			(
				from	match in this.GetAll()
				from	user in this.Context.Users
						join
						firstTeam in this.Context.Teams on match.FirstTeamID equals firstTeam.TeamID
						join
						secondTeam in this.Context.Teams on match.SecondTeamID equals secondTeam.TeamID
						join
							allBet in this.Context.MatchBets on new { MatchID = match.MatchID, UserID = user.UserID } equals new { MatchID = allBet.MatchID, UserID = allBet.UserID } into allBets
							from bet in allBets.DefaultIfEmpty()
				where	match.Date.Date == today && bet == null
				orderby user.UserID, match.Date
				select	new MatchWithNoBetModel
						{
							ID = match.MatchID,
							FirstTeamName = firstTeam.Name,
							SecondTeamName = secondTeam.Name,
							Points1 = match.Points1,
							PointsX = match.PointsX,
							Points2 = match.Points2,
							Date = match.Date,
							FirstTeamFlag = firstTeam.FlagPrefix,
							SecondTeamFlag = secondTeam.FlagPrefix,
							FirstTeamGoals = match.FirstTeamGoals,
							SecondTeamGoals = match.SecondTeamGoals,
							Result = match.Result,
							BetFirstTeamGoals = bet.FirstTeamGoals,
							BetSecondTeamGoals = bet.SecondTeamGoals,
							BetResult = bet.Result,
							BetPointsWon = bet.PointsWon,
							BetPointsWonBonus = bet.PointsWonBonus,
							Bonus = bet.Bonus,
							Expired = IsExpired(match.Date),
							WinTypeID = bet.WinTypeID ?? 0,
							UserID = user.UserID,
							UserEmail = user.Email,
							Username = user.Username
						}
			).ToList();

			var matchListByUser = new Dictionary<string, List<MatchWithNoBetModel>>();

			//group matches by user
			foreach (var match in matches)
			{
				if (match.Username != "admin")
				{
					string email = match.UserEmail;

					if (!matchListByUser.ContainsKey(email))
						matchListByUser[email] = new List<MatchWithNoBetModel>();

					matchListByUser[email].Add(match);
				}
			}

			return matchListByUser;
		}
	}
}
