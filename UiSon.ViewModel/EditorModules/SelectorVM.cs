// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UiSon.Element;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Selects from a drop down of strings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectorVM<T> : NPCBase, IEditorModule, ISelectorVM
    {
        public string Name { get; private set; }

        public int Priority { get; private set; }

        public string Value
        {
            get
            {
                if (_badValue) { return null; }

                var output = _element.GetValueAs(typeof(T));

                if (output == null) { return "null"; }

                return _converter.Reverse[(T)output];
            }
            set => SetValue(value);
        }

        public Visibility IsNameVisible => string.IsNullOrWhiteSpace(Name) ? Visibility.Collapsed : Visibility.Visible;

        public Brush TextColor => Value == "null" ? UiSonColors.Red : UiSonColors.Black;

        public IEnumerable<string> Options => _converter.Keys;

        private IUiSonElement _element;
        private Map<string, T> _converter;
        private bool _badValue = false;

        public SelectorVM(IUiSonElement element,
                          string name, int priority,
                          Map<string, T> converter)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

            Name = name;
            Priority = priority;

            _element.PropertyChanged += OnElementPropertyChanged;

            SetValue(Options.FirstOrDefault());
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

            var comboBox = new FrameworkElementFactory(typeof(ComboBox));
            var valBind = new Binding(path + $".{nameof(Value)}");
            valBind.Mode = BindingMode.TwoWay;
            valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            comboBox.SetBinding(ComboBox.SelectedValueProperty, valBind);
            comboBox.SetBinding(ComboBox.ItemsSourceProperty, new Binding(path + $".{nameof(Options)}"));
            comboBox.SetBinding(ComboBox.ForegroundProperty, new Binding(path + $".{nameof(TextColor)}"));

            dt.VisualTree = comboBox;

            valCol.CellTemplate = dt;

            return new List<DataGridColumn>() { valCol };
        }

        /// <summary>
        /// Writes this editor's element's value to the instance
        /// </summary>
        public void Read(object instance)
        {
            _element.Read(instance);

            // The _element could have read in a value not present in the converter. We need to set _badValue in that case so a bad value isn't looked up
            T readVal = (T)(_element.GetValueAs(typeof(T)) ?? (!typeof(T).IsValueType ? null : Activator.CreateInstance(typeof(T))));
            if (!_converter.Values.Any(x => EqualityComparer<T>.Default.Equals(x, readVal)))
            {
                // a bad value was read, oh no!
                // try setting it to null
                if (_element.SetValue(null))
                {
                    _badValue = false;
                }
                else
                {
                    // otherwise lets try to pick a value
                    foreach (var value in _converter.Values)
                    {
                        if (_element.SetValue(value))
                        {
                            // yay we found us a good 'un
                            _badValue = false;
                            return;
                        }
                    }

                    // mission failed, we'll get 'em next time
                    _badValue = true;
                }
            }
        }

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
            var strValue = value?.ToString();

            var result = false;

            if (strValue == null || strValue == "null")
            {
                result = _element.SetValue(null);
            }
            else if (_element.SetValue(_converter.Forward[strValue]))
            {
                result = true;

                // having been set from the converter's options, the value can no longer be bad
                _badValue = false;
            }

            return result;
        }

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

        public bool IdSetValue(object value)
        {
            if (value != null && value.GetType() == typeof(T))
            {
                return SetValue(_converter.Reverse[(T)value]);
            }
            return false;
        }
    }
}
