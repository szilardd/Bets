using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.IO;

namespace Bets.Data.Models
{
	public class FileTypesAttribute : ValidationAttribute
	{
		public string[] Allowed { get; set; }

		public FileTypesAttribute(string[] allowed)
		{
			this.Allowed = allowed;
		}

		public override bool IsValid(object value)
		{
			if (value == null)
				return true;

			var upload = (HttpPostedFileBase)value;

			return Allowed.Contains(Path.GetExtension(upload.FileName.ToLower()).Trim(new[] { '.' }));
		}
	}

}
