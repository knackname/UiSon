// UiSon, by Cameron Gale 2021

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using UiSon.Element;

namespace UiSon.Converter
{
    /// <summary>
    /// A converter with a defined brush for true and false. Converts a bool value to its defined brush.
    /// </summary>
    public class ModuleStateToBrushConverter : DependencyObject, IValueConverter
    {
        public Brush NormalBrush
        {
            get => (Brush)GetValue(NormalBrushProperty);
            set => SetValue(NormalBrushProperty, value);
        }
        public static readonly DependencyProperty NormalBrushProperty = DependencyProperty.Register("NormalBrush",
                                                                                                   typeof(Brush),
                                                                                                   typeof(ModuleStateToBrushConverter),
                                                                                                   new PropertyMetadata(null));

        public Brush ErrorBrush
        {
            get => (Brush)GetValue(ErrorBrushProperty);
            set => SetValue(ErrorBrushProperty, value);
        }

        public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register("ErrorBrush",
                                                                                                  typeof(Brush),
                                                                                                  typeof(ModuleStateToBrushConverter),
                                                                                                  new PropertyMetadata(null));

        public Brush SpecialBrush
        {
            get => (Brush)GetValue(SpecialBrushProperty);
            set => SetValue(SpecialBrushProperty, value);
        }

        public static readonly DependencyProperty SpecialBrushProperty = DependencyProperty.Register("SpecialBrush",
                                                                                                     typeof(Brush),
                                                                                                     typeof(ModuleStateToBrushConverter),
                                                                                                     new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ModuleState)value)
            {
                case ModuleState.Error:
                    return ErrorBrush;
                case ModuleState.Special:
                    return SpecialBrush;
            }

            return NormalBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
