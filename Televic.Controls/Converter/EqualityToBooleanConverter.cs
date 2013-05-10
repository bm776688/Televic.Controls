using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace Televic.Controls.Converter
{
	public class EqualityToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return object.Equals(value, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((bool)value)
				return parameter;

			throw new Exception("EqualityToBooleanConverter: It's false, I won't bind back.");
		}
	}
}
