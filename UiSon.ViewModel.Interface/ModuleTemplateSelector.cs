// UiSon, by Cameron Gale 2021

using System.Windows;
using System.Windows.Controls;
using UiSon.Attribute;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Selects the template from ElementEditor.xaml depending on the element type
    /// </summary>
    public class ModuleTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement frameworkElement)
            {
                if (item is ITextBlockModule)
                {
                    return frameworkElement.FindResource("TextBlockTemplate") as DataTemplate;
                }
                else if (item is IBorderedModule)
                {
                    return frameworkElement.FindResource("BorderedTemplate") as DataTemplate;
                }
                else if (item is IBorderedValueModule)
                {
                    return frameworkElement.FindResource("BorderedValueTemplate") as DataTemplate;
                }
                else if (item is INullableModule)
                {
                    return frameworkElement.FindResource("NullableTemplate") as DataTemplate;
                }
                else if (item is ISliderModule slider)
                {
                    return slider.IsVertical
                        ? frameworkElement.FindResource("SliderVerticalTemplate") as DataTemplate
                        : frameworkElement.FindResource("SliderHorizontalTemplate") as DataTemplate;
                }
                else if (item is ITextEditModule)
                {
                    return frameworkElement.FindResource("TextEditTemplate") as DataTemplate;
                }
                else if (item is ISelectorModule)
                {
                    return frameworkElement.FindResource("SelectorTemplate") as DataTemplate;
                }
                else if (item is ICheckboxModule)
                {
                    return frameworkElement.FindResource("CheckBoxTemplate") as DataTemplate;
                }
                else if (item is ICollectionModule collection)
                {
                    switch (collection.DisplayMode)
                    {
                        case DisplayMode.Vertial:
                            return frameworkElement.FindResource("CollectionVerticalTemplate") as DataTemplate;
                        case DisplayMode.Horizontal:
                            return frameworkElement.FindResource("CollectionHorizontalTemplate") as DataTemplate;
                        case DisplayMode.Wrap:
                            return frameworkElement.FindResource("CollectionWrapTemplate") as DataTemplate;
                        case DisplayMode.Grid:
                            return frameworkElement.FindResource("CollectionGridTemplate") as DataTemplate;
                    }
                }
                else if (item is IGroupModule group)
                {
                    switch (group.DisplayMode)
                    {
                        case DisplayMode.Vertial:
                            return frameworkElement.FindResource("GroupVerticalTemplate") as DataTemplate;
                        case DisplayMode.Horizontal:
                            return frameworkElement.FindResource("GroupHorizontalTemplate") as DataTemplate;
                        case DisplayMode.Wrap:
                            return frameworkElement.FindResource("GroupWrapTemplate") as DataTemplate;
                        case DisplayMode.Grid:
                            return frameworkElement.FindResource("GroupGridTemplate") as DataTemplate;
                    }
                }
            }

            return null;
        }
    }
}
