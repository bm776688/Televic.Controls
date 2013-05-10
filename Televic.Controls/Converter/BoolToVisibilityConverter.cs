﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Televic.Controls.Converter
{
	/// <summary>
	/// Implements the conversion between a <code>bool?</code> 
	/// and the <code>Visibility</code> enum:
	/// null  = Hidden
	/// false = Collapsed
	/// true  = Visible
	/// </summary>
	public class BoolToVisibilityConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool? visible = (bool?)value;
			if (visible.HasValue)
				return (visible.Value ? Visibility.Visible : Visibility.Collapsed);
			else
				return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Visibility vis = (Visibility)value;
			switch (vis)
			{
				case Visibility.Collapsed:
					return false;
				case Visibility.Hidden:
					return null;
				case Visibility.Visible:
					return true;
			}
			return null;
		}

		#endregion
	}
}