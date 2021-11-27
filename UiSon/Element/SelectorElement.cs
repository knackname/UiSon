// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UiSon.Element
{
    /// <summary>
    /// Selects from a defined list of options
    /// </summary>
    public class SelectorElement : StringElement
    {
        public IEnumerable<string> Options { get; private set; }

        public SelectorElement(string name, int priority, MemberInfo info, IEnumerable<string> options, string defaultValue)
            :base(name, priority, info)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Value = defaultValue;
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
