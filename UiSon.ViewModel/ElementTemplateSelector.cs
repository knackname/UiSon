// UiSon, by Cameron Gale 2021

using System.Windows;
using System.Windows.Controls;
using UiSon.Attribute;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Selects the template from ElementEditor.xaml depending on the element type
    /// </summary>
    public class ElementTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement frameworkElement)
            {
                if (item is BorderedVM)
                {
                    return frameworkElement.FindResource("BorderedTemplate") as DataTemplate;
                }
                else if (item is NullableVM)
                {
                    return frameworkElement.FindResource("NullableTemplate") as DataTemplate;
                }
                else if (item is TextEditVM)
                {
                    return frameworkElement.FindResource("TextEditElementTemplate") as DataTemplate;
                }
                else if (item is ISelectorVM)
                {
                    return frameworkElement.FindResource("SelectorElementTemplate") as DataTemplate;
                }
                else if (item is CheckboxVM)
                {
                    return frameworkElement.FindResource("CheckBoxElementTemplate") as DataTemplate;
                }
                // Sub types of GroupVM, must be identified first
                else if (item is ICollectionVM col)
                {
                    switch (col.Style)
                    {
                        case CollectionStyle.Stack:
                            switch (col.DisplayMode)
                            {
                                case DisplayMode.Vertial:
                                    return frameworkElement.FindResource("CollectionElementVerticalTemplate") as DataTemplate;
                                case DisplayMode.Horizontal:
                                    return frameworkElement.FindResource("CollectionElementHorizontalTemplate") as DataTemplate;
                                case DisplayMode.Wrap:
                                    return frameworkElement.FindResource("CollectionElementWrapTemplate") as DataTemplate;
                            }
                            break;
                        case CollectionStyle.Grid:
                            return frameworkElement.FindResource("GridCollectionTemplate") as DataTemplate;
                    }
                }
                else if (item is GroupVM group)
                {
                    switch (group.DisplayMode)
                    {
                        case DisplayMode.Vertial:
                            return frameworkElement.FindResource("VerticalGroupTemplate") as DataTemplate;
                        case DisplayMode.Horizontal:
                            return frameworkElement.FindResource("HorizontalGroupTemplate") as DataTemplate;
                        case DisplayMode.Wrap:
                            return frameworkElement.FindResource("WrapGroupTemplate") as DataTemplate;
                    }
                }
            }

            return null;
        }
    }
}
