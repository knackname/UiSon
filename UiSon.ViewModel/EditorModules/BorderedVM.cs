// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates an element with a boarder
    /// </summary>
    public class BorderedVM : NPCBase, IEditorModule
    {
        public string Name => _decorated.Name;

        public int Priority => _decorated.Priority;

        public Visibility IsNameVisible => _decorated.IsNameVisible;

        public IEditorModule Decorated => _decorated;
        private IEditorModule _decorated;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="decorated">The decorated module</param>
        public BorderedVM(IEditorModule decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));

            _decorated.PropertyChanged += OnDecoratedPropertyChanged;
        }

        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _decorated.GenerateColumns(path + $".{nameof(Decorated)}");

        public void Read(object instance) => _decorated.Read(instance);

        public void Write(object instance) => _decorated.Write(instance);

        public bool SetValue(object value) => _decorated.SetValue(value);

        public object GetValueAs(Type type) => _decorated.GetValueAs(type);

        public void UpdateRefs() => _decorated.UpdateRefs();

        private void OnDecoratedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case nameof(IEditorModule.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
                case nameof(IEditorModule.IsNameVisible):
                    OnPropertyChanged(nameof(IsNameVisible));
                    break;
                case nameof(IEditorModule.Priority):
                    OnPropertyChanged(nameof(Priority));
                    break;
            }
        }
    }
}
