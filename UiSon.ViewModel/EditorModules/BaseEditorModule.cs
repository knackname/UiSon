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
    public abstract class BaseEditorModule : NPCBase, IEditorModule
    {
        public virtual object Value
        {
            get => _badValue ?? _view.Value;
            set
            {
                if (Value != value)
                {
                    _badValue = _view.TrySetValue(value)
                        ? null
                        : value;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        private object _badValue;

        public string Name => _name;
        private string _name;

        public int DisplayPriority => _displayPriority;
        private readonly int _displayPriority;

        public virtual bool IsNameVisible => !string.IsNullOrWhiteSpace(Name);
        public virtual ModuleState State => _badValue == null ? ModuleState.Normal : ModuleState.Error;

        protected readonly IReadWriteView _view;
        private readonly ModuleTemplateSelector _templateSelector;

        public BaseEditorModule(IReadWriteView view, ModuleTemplateSelector templateSelector, string name, int displayPriority)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            view.PropertyChanged += OnViewPropertyChanged;

            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));

            _name = name;
            _displayPriority = displayPriority;
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ValueView<bool>.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        public void Read(object instance) => _view.Read(instance);

        public void Write(object instance) => _view.Write(instance);

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
