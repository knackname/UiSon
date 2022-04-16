// UiSon, by Cameron Gale 2021

using System.Windows;
using System.Windows.Controls;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Selects the template from ElementEditor.xaml depending on the element type
    /// </summary>
    public class DirectElementTemplateSelector : DataTemplateSelector
    {
        private string _templateName;

        public DirectElementTemplateSelector(string templateName)
        {
            _templateName = templateName;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (container is FrameworkElement frameworkElement)
            {
                return frameworkElement.FindResource(_templateName) as DataTemplate;
            }

            return null;
        }
    }
}
