using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Eddi.Common.WPF.Converter
{
	public class NullToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Inverse ? value != null : value == null) ? (ShowAsHidden ? Visibility.Hidden : Visibility.Collapsed) : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public bool ShowAsHidden { get; set; }
		public bool Inverse { get; set; }
	}
}
