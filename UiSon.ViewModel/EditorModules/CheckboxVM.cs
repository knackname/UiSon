// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UiSon.Element;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A checkbox for bool values
    /// </summary>
    public class CheckboxVM : NPCBase, IEditorModule
    {
        public string Name { get; private set; }

        public int Priority { get; private set; }

        public bool Value
        {
            get => (bool)(_element.Value ?? false);
            set => SetValue(value);
        }

        public Visibility IsNameVisible => string.IsNullOrWhiteSpace(Name) ? Visibility.Collapsed : Visibility.Visible;

        private ValueElement<bool> _element;

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckboxVM(ValueElement<bool> element, string name, int priority)
        {
            Name = name ?? "bool";
            Priority = priority;
            _element = element ?? throw new ArgumentNullException(nameof(element));

            _element.PropertyChanged += OnElementPropertyChanged;
        }

        /// <summary>
        /// Generates data grid column(s) for parent grids
        /// </summary>
        public IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridCheckBoxColumn();
            valCol.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            valCol.Header = Name;

            var valBinding = new Binding(path + $".{nameof(Value)}");
            valBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            valCol.Binding = valBinding;

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
        /// </summary>
        /// <param name="type">requested type</param>
        /// <returns>converted value</returns>
        public object GetValueAs(Type type) => _element.GetValueAs(type);

        public bool SetValue(object value) => _element.SetValue(value as bool?);

        public void UpdateRefs()
        {
            // no refs to update
        }

        private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ValueElement<bool>.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }
    }
}
