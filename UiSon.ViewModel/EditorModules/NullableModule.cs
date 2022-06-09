// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UiSon.Element;
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
            get => _view.IsNull ? null : _decorated.Value;
            set => _view.TrySetValue(value);
        }

        /// <inheritdoc/>
        public string Name => _view.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _view.DisplayPriority;

        /// <inheritdoc/>
        public bool IsNameVisible => !string.IsNullOrWhiteSpace(Name);

        /// <inheritdoc/>
        public ModuleState State => _view.IsValueBad 
            ? ModuleState.Error
            : _view.IsNull 
                ? ModuleState.Special
                : _decorated.State;

        /// <inheritdoc/>
        public bool IsExpanded
        {
            get => _isExpanded && !_view.IsNull;
            set
            {
                if (_isExpanded != value && !_view.IsNull)
                {
                    _isExpanded = value && !_view.IsNull;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isExpanded = false;

        /// <inheritdoc/>
        public bool IsNull
        {
            get => _view.IsNull;
            set => _view.IsNull = value;
        }

        /// <inheritdoc/>
        public string StateJustification => _view.IsNull ? "The value is null." : _decorated.StateJustification;

        /// <inheritdoc/>
        public IValueEditorModule Decorated => _decorated;
        private IValueEditorModule _decorated;

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly NullBufferValueView _view;

        private readonly ModuleTemplateSelector _templateSelector;
        private readonly EditorModuleFactory _factory;

        public NullableModule(NullBufferValueView view,
                              EditorModuleFactory factory,
                              ModuleTemplateSelector templateSelector)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;

            _factory = factory ?? throw new ArgumentNullException(nameof(_factory));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));

            if (!_view.IsNull)
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
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(NullBufferValueView.DisplayPriority):
                    OnPropertyChanged(nameof(DisplayPriority));
                    break;
                case nameof(NullBufferValueView.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
                case nameof(NullBufferValueView.IsValueBad):
                    OnPropertyChanged(nameof(State));
                    break;
                case nameof(NullBufferValueView.IsNull):
                    if (!_view.IsNull && _decorated == null)
                    {
                        _decorated = _factory.MakeUiValueEditorModule(_view.Decorated);
                        _decorated.PropertyChanged += OnDecoratedPropertyChanged;

                        OnPropertyChanged(nameof(Decorated));
                    }

                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(IsNull));
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(IsExpanded));
                    OnPropertyChanged(nameof(StateJustification));
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
    }
}
