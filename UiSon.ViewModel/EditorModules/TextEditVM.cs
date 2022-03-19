// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UiSon.Element;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A text editor for string values
    /// </summary>
    public class TextEditVM : NPCBase, IEditorModule
    {
        public string Name { get; private set; }

        public int Priority { get; private set; }

        public string Value
        {
            get => (string)_element.GetValueAs(typeof(string)) ?? "null";
            set => SetValue(value);
        }

        public Brush TextColor => Value == "null" ? UiSonColors.Red : UiSonColors.Black;

        public Visibility IsNameVisible => string.IsNullOrWhiteSpace(Name) ? Visibility.Collapsed : Visibility.Visible;

        private StringElement _element;

        /// <summary>
        /// Constructor
        /// </summary>
        public TextEditVM(StringElement element, string name, int priority)
        {
            Name = name;
            Priority = priority;
            _element = element ?? throw new ArgumentNullException(nameof(element));
        }

        /// <summary>
        /// Generates data grid column(s) for parent grids
        /// </summary>
        public IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var columns = new List<DataGridColumn>();

            var valCol = new DataGridTemplateColumn();
            valCol.Header = Name;

            var dt = new DataTemplate();

            var textBox = new FrameworkElementFactory(typeof(TextBox));
            var valBind = new Binding(path + $".{nameof(Value)}");
            valBind.Mode = BindingMode.TwoWay;
            valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            textBox.SetBinding(TextBox.TextProperty, valBind);
            textBox.SetBinding(TextBox.ForegroundProperty, new Binding(path + $".{nameof(TextColor)}"));

            dt.VisualTree = textBox;

            valCol.CellTemplate = dt;

            columns.Add(valCol);
            return columns;
        }

        /// <summary>
        /// Writes this editor's element's value to the instance
        /// </summary>
        public void Read(object instance) => _element.Read(instance);

        /// <summary>
        /// Reads data from instance and set's this editor's element's value to it
        /// </summary>
        public void Write(object instance) => _element.Write(instance);

        /// <summary>
        /// Returns the value as the resuested type.
        /// Returns null if failed to convert.
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>converted value</returns>
        public object GetValueAs(Type type) => _element.GetValueAs(type);

        public bool SetValue(object value)
        {
            if (_element.SetValue(value))
            {
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(TextColor));
                return true;
            }

            return false;
        }

        public void UpdateRefs()
        {
            // no refs to update
        }
    }
}
