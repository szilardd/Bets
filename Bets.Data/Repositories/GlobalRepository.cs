using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bets.Data.Models;
using System.Web.Mvc;

namespace Bets.Data
{
	public class GlobalRepository
	{
		private BetsDataContext context;
		public int UserID { get; set; }

		public GlobalRepository() {}

		public GlobalRepository(BetsDataContext context, int? userID = null)
		{
			this.context = context;
			if (userID != null)
				this.UserID = userID.Value;
		}
	}
}
