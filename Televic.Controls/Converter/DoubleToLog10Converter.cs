﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Televic.Controls.Converter
{
	public class DoubleToLog10Converter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double val = (double)value;
			return Math.Log10(val);
		}

		public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double val = (double)value;
			return Math.Pow(10, val);
		}

		#endregion
	}
}
