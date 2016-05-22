using System.Linq;
using Bets.Data.Models;
using Bets.Data;
using Bets.Helpers;

namespace Bets.Controllers
{
	public partial class RoundBetsController : BaseController
	{
		public RoundBetsController() : base(Module.RoundBets) { }

		protected override void SetIndexData()
		{
			base.SetIndexData();
			var rounds = new RoundRepository().GetClosedRounds(DataExtensions.UserIsAdmin());
			ViewData.Model = rounds.GetEnumerator();
			
			if (rounds.Count() > 0)
				ViewBag.FirstRoundID = rounds.First().ID;
		}
	}
}
