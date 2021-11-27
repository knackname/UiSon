// UiSon, by Cameron Gale 2021

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UiSon.ViewModel;

namespace UiSon.Element
{
    /// <summary>
    /// Selects from a base collection and an collection of element names
    /// </summary>
    public class ElementSelectorElement : StringElement
    {
        public IEnumerable<string> Options => BaseOptions.Concat(ElementVMs.Select(x => x.Name));

        private string[] BaseOptions;
        private ObservableCollection<ElementVM> ElementVMs;

        public ElementSelectorElement(string name, int priority, MemberInfo info, string[] baseOptions, ObservableCollection<ElementVM> elementVMs)
            :base(name, priority, info)
        {
            BaseOptions = baseOptions ?? new string[] { };
            ElementVMs = elementVMs;

            elementVMs.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(Options)); 
                if (Value == null)
                {
                    Value = Options.FirstOrDefault();
                }
            };

            Value = Options.FirstOrDefault();
        }

        public override IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var columns = new List<DataGridColumn>();

            var valCol = new DataGridTemplateColumn();
            valCol.Header = Name;

            var dt = new DataTemplate();

            var comboBox = new FrameworkElementFactory(typeof(ComboBox));
            var valBind = new Binding(path + ".Value");
            valBind.Mode = BindingMode.TwoWay;
            valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            comboBox.SetBinding(ComboBox.SelectedValueProperty, valBind);
            comboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding(path + ".Options"));

            dt.VisualTree = comboBox;

            valCol.CellTemplate = dt;

            columns.Add(valCol);

            return columns;
        }
    }
}
