// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UiSon.Element;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class SliderVM : NPCBase, IEditorModule
    {
        public string Name { get; private set; }

        public int Priority { get; private set; }

        public double Value
        {
            get => (double)(_element.GetValueAs(typeof(double)) ?? -1d);
            set => SetValue(value);
        }

        public double Min => _element.Min;

        public double Max => _element.Max;

        public bool IsVertical { get; private set; }

        public Visibility IsNameVisible => string.IsNullOrWhiteSpace(Name) ? Visibility.Collapsed : Visibility.Visible;

        private RangeElement _element;
        private ElementTemplateSelector _templateSelector;

        public SliderVM(RangeElement element, bool isVertical, string name, int priority, ElementTemplateSelector templateSelector)
        {
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));

            IsVertical = isVertical;
            Priority = priority;
            Name = name;

            _element.PropertyChanged += Refresh;
        }

        public IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridTemplateColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = Name
            };

            var dt = new DataTemplate();
            var valBind = new Binding(path)
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

        public object GetValueAs(Type type) => _element.GetValueAs(type);

        public void Read(object instance) => _element.Read(instance);

        public void Write(object instance) => _element.Write(instance);

        public bool SetValue(object value) => _element.SetValue(value);

        public void UpdateRefs()
        {
            // no refs to update
        }

        private void Refresh(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsNameVisible));
            OnPropertyChanged(nameof(Value));
        }
    }
}
