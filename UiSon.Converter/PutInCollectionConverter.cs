using System;
using System.Windows.Data;
using System.Collections.Generic;

namespace UiSon.Converter
{
	public class PutInCollectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new List<object>() { value };
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (value as List<object>)[0];
		}
	}
}
