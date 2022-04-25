// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private IUiSonElement _element;

        /// <summary>
        /// Constructor
        /// </summary>
        public TextEditVM(IUiSonElement element, string name, int priority)
        {
            Name = name;
            Priority = priority;
            _element = element ?? throw new ArgumentNullException(nameof(element));

            _element.PropertyChanged += OnElementPropertyChanged;
        }

        /// <summary>
        /// Generates data grid column(s) for parent grids
        /// </summary>
        public IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridTemplateColumn();
            valCol.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
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

            return new List<DataGridColumn>() { valCol };
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

        public bool SetValue(object value) => _element.SetValue(value);

        public void UpdateRefs()
        {
            // no refs to update
        }

        private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiSonElement.Value):
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(TextColor));
                    break;
            }
        }
    }
}
