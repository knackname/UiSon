// UiSon, by Cameron Gale 2021

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
    /// Decorates another module with a border.
    /// </summary>
    public class BorderedModule : NPCBase, IDecoratingModule, IBorderedModule
    {
        /// <inheritdoc/>
        public string Name => _decorated.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public ModuleState State => _decorated.State;

        /// <inheritdoc/>
        public string StateJustification => _decorated.StateJustification;

        /// <inheritdoc/>
        public IEditorModule Decorated => _decorated;
        private readonly IEditorModule _decorated;

        public BorderedModule(IEditorModule decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;
        }

        private void OnDecoratedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IValueEditorModule.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
                case nameof(IValueEditorModule.DisplayPriority):
                    OnPropertyChanged(nameof(DisplayPriority));
                    break;
                case nameof(IValueEditorModule.State):
                    OnPropertyChanged(nameof(State));
                    break;
                case nameof(IValueEditorModule.View):
                    OnPropertyChanged(nameof(View));
                    break;
                case nameof(IValueEditorModule.StateJustification):
                    OnPropertyChanged(nameof(StateJustification));
                    break;
                case nameof(IValueEditorModule.Value):
                    OnPropertyChanged(nameof(IValueEditorModule.Value));
                    break;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _decorated.GenerateColumns(path + $".{nameof(Decorated)}");
    }
}
