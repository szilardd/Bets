using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Bets.Data
{
	public class DropdownOption : SelectListItem
	{
		public string CustomData { get; set; }
		public string CustomData2 { get; set; }
	}
}
