using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bets.Data.Models;
using Bets.Data;

namespace Bets.Models
{
	public enum DetailAction
	{
		None = 0,
		Add = 1,		//action after adding a new item
		Update = 2,		//action after editing an item
		Close = 3,		//action after closing an item
		Delete = 4,		//action after deleteing an item
		AddClose = 5,	//action after closing the 'add page' for an item
	}

	/// <summary>
	/// ViewModel passed to detail pages
	/// </summary>
	public class DetailViewModel
	{
		//permission module
		public Module Module { get; set; }

		//parent module of current page (optional); used mainly in sub-detail pages
		public Module ParentModule { get; set; }

		//additional parameters which link the current item with its master record
		public Dictionary<string, object> Parameters { get; set; }

		//user friendly display label
		public string Label { get; set; }

		//if true, allows delete and shows 'Delete' button
		public bool AllowDelete { get; set; }
		public string PageType { get; set; }

		public string SelectedTab { get; set; }

		//urls to be called after completing a specific action (Add, Update, Close Record)
		public Dictionary<DetailAction, ControllerAction> Actions { get; set; }

		//what detail action caused the page load
		public DetailAction Action { get; set; }

		public IModel SecondaryModel { get; set; }

		public DetailViewModel()
		{
			this.Actions = new Dictionary<DetailAction, ControllerAction>();

			//allow delete by default
			this.AllowDelete = true;

			//default close action loads the index page; can be overridden in 'SetDetailData'
			this.Actions[DetailAction.Close] = new ControllerAction { Action = "Index", Controller = this.PageType };

			//default close action on 'add' page; loads the index page
			this.Actions[DetailAction.AddClose] = new ControllerAction { Action = "Close", Controller = this.PageType };
		}

		
	}
}