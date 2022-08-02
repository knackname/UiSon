// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UiSon.Command;
using UiSon.Element;
using UiSon.Notify.Interface;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// basic implimentation of <see cref="IEditorModule"/>
    /// </summary>
    public abstract class BaseEditorModule : NPCBase, IValueEditorModule
    {
        /// <inheritdoc/>
        public virtual object Value
        {
            get => _view.DisplayValue;
            set
            {
                if (_view.DisplayValue != value)
                {
                    _view.SetValue(value);
                }
            }
        }

        /// <inheritdoc/>
        public abstract Type ValueType { get; }

        /// <inheritdoc/>
        public string Name => _view.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _view.DisplayPriority;

        /// <inheritdoc/>
        public virtual bool IsNameVisible => !string.IsNullOrWhiteSpace(Name);

        /// <inheritdoc/>
        public virtual ModuleState State => _view.State;

        /// <inheritdoc/>
        public string StateJustification => _view.StateJustification;

        /// <inheritdoc/>
        public bool HasError => _view.State == ModuleState.Error;

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly IUiValueView _view;
        
        private readonly ModuleTemplateSelector _templateSelector;
        private readonly ClipBoardManager _clipBoardManager;
        private readonly INotifier _notifier;

        public BaseEditorModule(IUiValueView view, ModuleTemplateSelector templateSelector, ClipBoardManager clipBoardManager, INotifier notifier)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            view.PropertyChanged += OnViewPropertyChanged;

            _clipBoardManager = clipBoardManager ?? throw new ArgumentNullException(nameof(clipBoardManager));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
            _clipBoardManager = clipBoardManager ?? throw new ArgumentNullException(nameof(clipBoardManager));
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.DisplayValue):
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IUiValueView.State):
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(HasError));
                    break;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<DataGridColumn> GenerateColumns(string bindingPath)
        {
            var valCol = new DataGridTemplateColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = Name
            };

            var dt = new DataTemplate();
            var valBind = new Binding(bindingPath)
            {
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, valBind);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, _templateSelector);

            dt.VisualTree = contentPresenter;
            valCol.CellTemplate = dt;

            return new List<DataGridColumn>() { valCol };
        }

        #region Commands

        /// <inheritdoc/>
        public ICommand CopyCommand => new UiSonActionCommand((s) => _clipBoardManager.Copy(this));

        /// <inheritdoc/>
        public ICommand PasteCommand => new UiSonActionCommand((s) => _clipBoardManager.Paste(this));

        /// <inheritdoc/>
        public ICommand ShowErrorCommand => new UiSonActionCommand((s) => _notifier.Notify(_view.StateJustification, "Error"));

        #endregion
    }
}
