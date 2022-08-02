// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using System.Windows.Input;
using UiSon.Command;
using UiSon.Notify.Interface;
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
            set => _view.SetValue(value);
        }

        /// <inheritdoc/>
        public Type ValueType => _view.Type;

        /// <inheritdoc/>
        public IUiValueView View => _view;

        /// <inheritdoc/>
        public bool HasError => throw new NotImplementedException();

        private readonly IEncapsulatingView _view;
        private readonly ClipBoardManager _clipBoardManager;
        private readonly INotifier _notifier;

        public EncapsulatingModule(IEncapsulatingView view,
                                   ClipBoardManager clipBoardManager,
                                   INotifier notifier,
                                   IEditorModule[] members)
            : base(view.Name, view.DisplayPriority, view.DisplayMode, members)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;
            _clipBoardManager = clipBoardManager ?? throw new ArgumentNullException(nameof(clipBoardManager));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(EncapsulatingView.DisplayValue):
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(HasError));
                    break;
            }
        }

        /// <inheritdoc/>
        public ICommand CopyCommand => new UiSonActionCommand((s) => _clipBoardManager.Copy(this));

        /// <inheritdoc/>
        public ICommand PasteCommand => new UiSonActionCommand((s) => _clipBoardManager.Paste(this));

        /// <inheritdoc/>
        public ICommand ShowErrorCommand => new UiSonActionCommand((s) => _notifier.Notify(_view.StateJustification, "Error"));
    }
}
