using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Bets.Data.Models
{
	public class MaxSizeAttribute : ValidationAttribute
	{
		public int MaxSize { get; set; }

		public MaxSizeAttribute(int maxSize)
		{
			this.MaxSize = maxSize;
		}

		public override bool IsValid(object value)
		{
			if (value == null)
				return true;

			var upload = (HttpPostedFileBase)value;

			return upload.ContentLength <= MaxSize * 1024;
		}
	}

}
