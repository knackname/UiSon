// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class EncapsulatingModule : GroupModule, IValueEditorModule
    {
        /// <inheritdoc/>
        public object Value
        {
            get => _view.DisplayValue;
            set => _view.TrySetValue(value);
        }

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly EncapsulatingView _view;

        public EncapsulatingModule(EncapsulatingView view,
                                   IEditorModule[] members)
            : base(view.Name, view.DisplayPriority, view.DisplayMode, members)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            { 
                case nameof(EncapsulatingView.DisplayValue):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }
    }
}
