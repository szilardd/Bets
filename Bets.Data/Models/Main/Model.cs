using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.ComponentModel.DataAnnotations;

namespace Bets.Data.Models
{
	public class Model : IModel
	{
		[Required]
		public virtual int ID { get; set; }

		public Module ParentModule { get; set; }
		public string ParentModuleName { get; set; }
		public object ParentID { get; set; }

		[Timestamp]
		public Binary Timestamp { get; set; }

		/// <summary>
		/// Returns the listing headers
		/// </summary>
		public virtual Dictionary<string, string> GetHeaders()
		{
			return null;
		}

		/// <summary>
		/// Returns the lookup listing headers
		/// </summary>
		public virtual Dictionary<string, string> GetLookupHeaders()
		{
			return null;
		}

		/// <summary>
		/// Returns the data to be displayed in the listing page for the current model
		/// </summary>
		public virtual object[] GetListingData(Module parentModule)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the data to be displayed in the lookup listing page for the current model
		/// </summary>
		public virtual object[] GetLookupListingData()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns data to be shown in the preview tooltip
		/// </summary>
		/// <returns></returns>
		public virtual Dictionary<string, object> GetPreviewData()
		{
			return null;
		}

		/// <summary>
		/// Returns route parameters which uniquely identify the model (if it depends on a master record or other parameters)
		/// </summary>
		public virtual object GetRouteParameters(Module parentModule)
		{
			return new { id = this.ID };
		}

		/// <summary>
		/// Sets sub-type of model, like Customer-Note or SalesCall-Order
		/// </summary>
		public virtual void SetMetaData()
		{

		}

		/// <summary>
		/// Retrurns the title of the model (to be shown in breadcrumbs)
		/// </summary>
		/// <returns></returns>
		public virtual string GetTitle()
		{
			return "Breadcrumb";
		}
	}

	public interface IModel
	{
		[Required]
		int ID { get; set; }

		[Timestamp]
		Binary Timestamp { get; set; }

		Dictionary<string, string> GetHeaders();
		Dictionary<string, string> GetLookupHeaders();
		object[] GetListingData(Module parentModule);
		object[] GetLookupListingData();
		Dictionary<string, object> GetPreviewData();
		object GetRouteParameters(Module parentModule);
		string GetTitle();
		void SetMetaData();
	}
}
