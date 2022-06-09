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
    /// basic implimentation of <see cref="IEditorModule"/>
    /// </summary>
    public abstract class BaseEditorModule : NPCBase, IValueEditorModule
    {
        /// <inheritdoc/>
        public virtual object Value
        {
            get => _isValueBad ? _badValue : _view.DisplayValue;
            set
            {
                if (_view.DisplayValue != value)
                {
                    _isValueBad = !_view.TrySetValue(value);

                    _badValue = _isValueBad ? value : null;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        private object _badValue;
        private bool _isValueBad;

        /// <inheritdoc/>
        public string Name => _view.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _view.DisplayPriority;

        /// <inheritdoc/>
        public virtual bool IsNameVisible => !string.IsNullOrWhiteSpace(Name);

        /// <inheritdoc/>
        public virtual ModuleState State
        {
            get
            {
                if (_isValueBad || _view.IsValueBad)
                {
                    _stateJustification = "Invalid value.";
                    return ModuleState.Error;
                }

                if (Value == null || (Value as string) == "null")
                {
                    _stateJustification = "Value is null.";
                    return ModuleState.Special;
                }

                _stateJustification = null;
                return ModuleState.Normal;
            }
        }

        /// <inheritdoc/>
        public string StateJustification => _stateJustification;
        private string _stateJustification;

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly IUiValueView _view;
        
        private readonly ModuleTemplateSelector _templateSelector;

        public BaseEditorModule(IUiValueView view, ModuleTemplateSelector templateSelector)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            view.PropertyChanged += OnViewPropertyChanged;

            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.DisplayValue):
                    _badValue = null;
                    _isValueBad = false;
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IUiValueView.IsValueBad):
                    OnPropertyChanged(nameof(State));
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
    }
}
