// UiSon, by Cameron Gale 2021

using System;
using System.Diagnostics;
using System.Windows.Data;

namespace UiSon.Converter
{
    /// <summary>
    /// For debugging, provides a place to set breakpoints.
    /// </summary>
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
