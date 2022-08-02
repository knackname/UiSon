// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace UiSon.Converter
{
    /// <summary>
    /// Converts an object to a list containg that object.
    /// </summary>
    public class PutInListConverter : IValueConverter
    {
        /// <inheritdoc cref="IValueConverter.Convert(object, Type, object, System.Globalization.CultureInfo)" />
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new List<object>() { value };
        }

        /// <inheritdoc cref="IValueConverter.ConvertBack(object, Type, object, System.Globalization.CultureInfo)" />
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as List<object>)[0];
        }
    }
}
