using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.Mail;

namespace Bets.Data
{
	public class NotificationRepository
	{
		private BetsDataContext context;

		public NotificationRepository()
		{
			this.context = new BetsDataContext();
		}

		public void SendMatchNotification()
		{
			
		}
	}
}
