using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Bets.Data;

namespace Bets.Infrastructure
{
	public class DateModelBinder : DefaultModelBinder
	{
		public DateModelBinder()
		{

		}

		protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
		{
			if (propertyDescriptor.PropertyType == typeof(DateTime?))
			{
				try
				{
					var model = bindingContext.Model;
					PropertyInfo property = model.GetType().GetProperty(propertyDescriptor.Name);

					var value = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);

					if (value != null)
					{
						DateTime date;

						if (DateTime.TryParseExact(value.AttemptedValue, DataConfig.DateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
							property.SetValue(model, date, null);
						else if (DateTime.TryParseExact(value.AttemptedValue, DataConfig.DateTimeFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
							property.SetValue(model, date, null);
						else if (DateTime.TryParse(value.AttemptedValue, out date))
							property.SetValue(model, date, null);
					}
				}
				catch
				{
					//If something wrong, validation should take care
				}
			}
			else
			{
				base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
			}
		}
	}
}