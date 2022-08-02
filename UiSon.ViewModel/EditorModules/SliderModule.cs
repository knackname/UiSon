// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using UiSon.Notify.Interface;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class SliderModule : BaseEditorModule, ISliderModule
    {
        /// <inheritdoc/>
        public double Min => _view.Min;

        /// <inheritdoc/>
        public double Max => _view.Max;

        /// <inheritdoc/>
        public bool IsVertical => _view.IsVertical;

        /// <inheritdoc/>
        public override Type ValueType => typeof(double);

        private readonly IRangeView _view;

        public SliderModule(IRangeView view,
                            ModuleTemplateSelector templateSelector,
                            ClipBoardManager clipBoardManager,
                            INotifier notifier)
            : base(view, templateSelector, clipBoardManager, notifier)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            _view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IRangeView.Min):
                    OnPropertyChanged(nameof(Min));
                    break;
                case nameof(IRangeView.Max):
                    OnPropertyChanged(nameof(Max));
                    break;
                case nameof(IRangeView.IsVertical):
                    OnPropertyChanged(nameof(IsVertical));
                    break;
            }
        }
    }
}
