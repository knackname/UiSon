// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class SliderModule : BaseEditorModule, ISliderModule
    {
        public double Min => _view.Min;

        public double Max => _view.Max;

        public bool IsVertical { get; private set; }

        private new readonly RangeView _view;
        public SliderModule(RangeView view,
                            ModuleTemplateSelector templateSelector,
                            string name,
                            int displayPriority,
                            bool isVertical)
            :base(view, templateSelector, name, displayPriority)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            IsVertical = isVertical;

            _view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(RangeView.Min):
                    OnPropertyChanged(nameof(Min));
                    break;
                case nameof(RangeView.Max):
                    OnPropertyChanged(nameof(Max));
                    break;
            }
        }
    }
}
