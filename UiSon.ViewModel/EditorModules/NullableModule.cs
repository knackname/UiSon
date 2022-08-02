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
    /// A special kind of border including a "Null" checkbox to allow the user to set the value to null.
    /// Decorates other <see cref="IEditorModule"/>.
    /// </summary>
    public class NullableModule : NPCBase, INullableModule
    {
        /// <inheritdoc/>
        public object Value
        {
            get => _decorated.Value;
            set => _view.SetValue(value);
        }

        /// <inheritdoc/>
        public Type ValueType => _view.Type;

        /// <inheritdoc/>
        public string Name => _view.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _view.DisplayPriority;

        /// <inheritdoc/>
        public bool IsNameVisible => !string.IsNullOrWhiteSpace(Name);

        /// <inheritdoc/>
        public ModuleState State => _view.State;

        /// <inheritdoc/>
        public bool HasError => _view.State == ModuleState.Error;

        /// <inheritdoc/>
        public bool IsExpanded
        {
            get => _isExpanded && !IsNull;
            set
            {
                if (_isExpanded != value && !IsNull)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isExpanded = false;

        /// <inheritdoc/>
        public bool IsNull
        {
            get => _view.DisplayValue?.ToString() == "null";
            set
            {
                if (value == true)
                {
                    _view.SetValue(null);
                    _isExpanded = false;
                }
                else
                {
                    _view.UnNull();
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        /// <inheritdoc/>
        public string StateJustification => _view.StateJustification;

        /// <inheritdoc/>
        public IValueEditorModule Decorated => _decorated;
        private IValueEditorModule _decorated;

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly NullBufferValueView _view;

        private readonly ModuleTemplateSelector _templateSelector;
        private readonly EditorModuleFactory _factory;
        private readonly ClipBoardManager _clipBoardManager;
        private readonly INotifier _notifier;

        public NullableModule(NullBufferValueView view,
                              EditorModuleFactory factory,
                              ClipBoardManager clipBoardManager,
                              ModuleTemplateSelector templateSelector,
                              INotifier notifier)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;

            _factory = factory ?? throw new ArgumentNullException(nameof(_factory));
            _clipBoardManager = clipBoardManager ?? throw new ArgumentNullException(nameof(_clipBoardManager));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            if (_view.DisplayValue?.ToString() != "null")
            {
                _decorated = _factory.MakeUiValueEditorModule(_view.Decorated);
                _decorated.PropertyChanged += OnDecoratedPropertyChanged;
            }
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(NullBufferValueView.Value):
                    if (_view.Value != null
                        && _decorated == null)
                    {
                        _decorated = _factory.MakeUiValueEditorModule(_view.Decorated);
                        _decorated.PropertyChanged += OnDecoratedPropertyChanged;

                        OnPropertyChanged(nameof(Decorated));
                    }
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(NullBufferValueView.DisplayPriority):
                    OnPropertyChanged(nameof(DisplayPriority));
                    break;
                case nameof(NullBufferValueView.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
                case nameof(NullBufferValueView.State):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        private void OnDecoratedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IValueEditorModule.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IValueEditorModule.State):
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(HasError));
                    break;
                case nameof(IValueEditorModule.StateJustification):
                    OnPropertyChanged(nameof(StateJustification));
                    break;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridTemplateColumn();
            valCol.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            valCol.Header = Name;

            var dt = new DataTemplate();
            var valBind = new Binding(path);
            valBind.Mode = BindingMode.OneWay;
            valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, valBind);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, _templateSelector);

            dt.VisualTree = contentPresenter;
            valCol.CellTemplate = dt;

            return new List<DataGridColumn>() { valCol };
        }

        #region Commands

        public ICommand CopyCommand => new UiSonActionCommand((s) => _clipBoardManager.Copy(this));
        public ICommand PasteCommand => new UiSonActionCommand((s) => _clipBoardManager.Paste(this));
        public ICommand ShowErrorCommand => new UiSonActionCommand((s) => _notifier.Notify(_view.StateJustification, "Error"));

        #endregion
    }
}
