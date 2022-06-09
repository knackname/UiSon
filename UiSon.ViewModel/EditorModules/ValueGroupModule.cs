// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using UiSon.Attribute;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A group of editor modules all sharing the same view
    /// </summary>
    public class ValueGroupModule : GroupModule, IValueEditorModule
    {
        /// <inheritdoc/>
        public object Value
        {
            get => _view.DisplayValue;
            set => _view.TrySetValue(value);
        }

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly IUiValueView _view;

        public ValueGroupModule(string name,
                                int displayPriority,
                                DisplayMode displayMode,
                                IUiValueView view,
                                IEditorModule[] members)
            : base(name, displayPriority, displayMode, members)
        {
            _view = view ?? throw new ArgumentNullException(nameof(name));
            _view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.DisplayValue):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }
    }
}
