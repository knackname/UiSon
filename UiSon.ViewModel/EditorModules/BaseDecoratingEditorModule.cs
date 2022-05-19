// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using UiSon.Element;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates another view model
    /// </summary>
    public abstract class BaseDecoratingEditorModule : NPCBase, IEditorModule
    {
        public virtual object Value
        {
            get => _decorated.Value;
            set => _decorated.Value = value;
        }

        public string Name => _decorated.Name;

        public int DisplayPriority => _decorated.DisplayPriority;

        public bool IsNameVisible => _decorated.IsNameVisible;

        public ModuleState State => _decorated.State;

        public IEditorModule Decorated => _decorated;
        private readonly IEditorModule _decorated;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="decorated">The decorated module.</param>
        public BaseDecoratingEditorModule(IEditorModule decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            decorated.PropertyChanged += OnDecoratedPropertyChanged;
        }

        private void OnDecoratedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IEditorModule.Value):
                case nameof(IEditorModule.Name):
                case nameof(IEditorModule.DisplayPriority):
                case nameof(IEditorModule.IsNameVisible):
                case nameof(IEditorModule.State):
                    OnPropertyChanged(e.PropertyName);
                    break;
            }
        }

        public void Read(object instance) => _decorated.Read(instance);

        public void Write(object instance) => _decorated.Write(instance);

        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _decorated.GenerateColumns(path + $".{nameof(Decorated)}");
    }
}
