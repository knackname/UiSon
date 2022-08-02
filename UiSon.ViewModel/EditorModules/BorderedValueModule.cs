// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Element;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates a <see cref="IValueEditorModule"/> with a border
    /// </summary>
    public class BorderedValueModule : NPCBase, IBorderedValueModule
    {
        /// <inheritdoc/>
        public object Value
        {
            get => _decorated.Value;
            set => _decorated.Value = value;
        }

        /// <inheritdoc/>
        public Type ValueType => _decorated.ValueType;

        /// <inheritdoc/>
        public string Name => _decorated.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public ModuleState State => ModuleState.Normal;

        /// <inheritdoc/>
        public string StateJustification => string.Empty;

        /// <inheritdoc/>
        public IUiValueView View => _decorated.View;

        /// <inheritdoc/>
        public IEditorModule Decorated => _decorated;

        /// <inheritdoc/>
        public bool HasError => false;

        private readonly IValueEditorModule _decorated;

        public BorderedValueModule(IValueEditorModule decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;
        }

        private void OnDecoratedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IValueEditorModule.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IValueEditorModule.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
                case nameof(IValueEditorModule.DisplayPriority):
                    OnPropertyChanged(nameof(DisplayPriority));
                    break;
                case nameof(IValueEditorModule.State):
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(HasError));
                    break;
                case nameof(IValueEditorModule.View):
                    OnPropertyChanged(nameof(View));
                    break;
                case nameof(IValueEditorModule.StateJustification):
                    OnPropertyChanged(nameof(StateJustification));
                    break;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _decorated.GenerateColumns(path + $".{nameof(Decorated)}");

        public ICommand CopyCommand => null;
        public ICommand PasteCommand => null;
        public ICommand ShowErrorCommand => null;
    }
}
