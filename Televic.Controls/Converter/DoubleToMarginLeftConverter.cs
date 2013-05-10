using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Televic.Controls.Converter
{
	class DoubleToMarginLeftConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double val = (double)value;
			Thickness thickness = new Thickness(val, 0, 0, 0);
			return thickness;
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Thickness thickness = (Thickness)value;
			if (thickness == null) return 0;
			else return thickness.Left;
		}

		#endregion
	}
}
