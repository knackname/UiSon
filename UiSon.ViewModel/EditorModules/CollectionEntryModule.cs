// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Command;
using UiSon.Element;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates another <see cref="IValueEditorModule"/> to act as a collection entry
    /// </summary>
    public class CollectionEntryModule : NPCBase, IDecoratingModule, ICollectionEntryModule
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
        public ModuleState State => _decorated.State;

        /// <inheritdoc/>
        public string StateJustification => _decorated.StateJustification;

        /// <inheritdoc/>
        public bool CanModifyCollection => _parent.CanModifyCollection;

        /// <inheritdoc/>
        public IUiValueView View => _decorated.View;

        /// <inheritdoc/>
        public bool HasError => _decorated.HasError;

        /// <inheritdoc/>
        public IEditorModule Decorated => _decorated;
        private readonly IValueEditorModule _decorated;

        private readonly CollectionModule _parent;

        public CollectionEntryModule(CollectionModule parent, IValueEditorModule decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;

            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
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

        #region Commands

        /// <inheritdoc/>
        public ICommand RemoveElement => new UiSonActionCommand((s) => _parent.RemoveEntry(this));

        /// <inheritdoc/>
        public ICommand CopyCommand => _decorated.CopyCommand;

        /// <inheritdoc/>
        public ICommand PasteCommand => _decorated.PasteCommand;

        /// <inheritdoc/>
        public ICommand ShowErrorCommand => _decorated.ShowErrorCommand;

        #endregion
    }
}
