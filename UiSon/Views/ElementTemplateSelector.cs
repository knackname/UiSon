// UiSon, by Cameron Gale 2021

using System.Windows;
using System.Windows.Controls;
using UiSon.Attribute.Enums;
using UiSon.Element;
using UiSon.Element.Element.Interface;

namespace UiSon.Views
{
    public class ElementTemplateSelector : DataTemplateSelector
    {
        public DataTemplate a { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement frameworkElement)
            {
                if (item is TextEditElement)
                {
                    return frameworkElement.FindResource("TextEditElementTemplate") as DataTemplate;
                }
                else if (item is SelectorElement || item is ElementSelectorElement)
                {
                    return frameworkElement.FindResource("SelectorElementTemplate") as DataTemplate;
                }
                else if (item is CheckboxElement)
                {
                    return frameworkElement.FindResource("CheckBoxElementTemplate") as DataTemplate;
                }
                else if (item is GroupElement group)
                {
                    switch (group.GroupType)
                    {
                        case GroupType.Expander:
                            switch (group.DisplayMode)
                            {
                                case DisplayMode.Vertial:
                                    return frameworkElement.FindResource("ExpanderElementVerticalTemplate") as DataTemplate;
                                case DisplayMode.Horizontal:
                                    return frameworkElement.FindResource("ExpanderElementHorizontalTemplate") as DataTemplate;
                                case DisplayMode.Wrap:
                                    return frameworkElement.FindResource("ExpanderElementWrapTemplate") as DataTemplate;
                            }
                            break;
                        case GroupType.Border:
                            switch (group.DisplayMode)
                            {
                                case DisplayMode.Vertial:
                                    return frameworkElement.FindResource("BorderElementVerticalTemplate") as DataTemplate;
                                case DisplayMode.Horizontal:
                                    return frameworkElement.FindResource("BorderElementHorizontalTemplate") as DataTemplate;
                                case DisplayMode.Wrap:
                                    return frameworkElement.FindResource("BorderElementWrapTemplate") as DataTemplate;
                            }
                            break;
                        case GroupType.Basic:
                            switch (group.DisplayMode)
                            {
                                case DisplayMode.Vertial:
                                    return frameworkElement.FindResource("GroupElementVerticalTemplate") as DataTemplate;
                                case DisplayMode.Horizontal:
                                    return frameworkElement.FindResource("GroupElementHorizontalTemplate") as DataTemplate;
                                case DisplayMode.Wrap:
                                    return frameworkElement.FindResource("GroupElementWrapTemplate") as DataTemplate;
                            }
                            break;
                    }
                }
                else if (item is ICollectionElement col)
                {
                    switch (col.Type)
                    {
                        case CollectionType.Template:
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
                        case CollectionType.Grid:
                            return frameworkElement.FindResource("GridCollectionTemplate") as DataTemplate;
                    }
                }
            }

            return null;
        }
    }
}
