// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Command;
using UiSon.Notify.Interface;
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
            set => _view.SetValue(value);
        }

        /// <inheritdoc/>
        public bool HasError => _view.State == Element.ModuleState.Error;

        /// <inheritdoc/>
        public Type ValueType => _view.Type;

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly IUiValueView _view;
        private readonly ClipBoardManager _clipBoardManager;
        private readonly INotifier _notifier;

        public ValueGroupModule(string name,
                                int displayPriority,
                                DisplayMode displayMode,
                                ClipBoardManager clipBoardManager,
                                INotifier notifier,
                                IUiValueView view,
                                IEditorModule[] members)
            : base(name, displayPriority, displayMode, members)
        {
            _clipBoardManager = clipBoardManager ?? throw new ArgumentNullException(nameof(clipBoardManager));
            _view = view ?? throw new ArgumentNullException(nameof(name));
            _view.PropertyChanged += OnViewPropertyChanged;
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.DisplayValue):
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(HasError));
                    break;
            }
        }

        #region Commands

        public ICommand CopyCommand => new UiSonActionCommand((s) => _clipBoardManager.Copy(this));
        public ICommand PasteCommand => new UiSonActionCommand((s) => _clipBoardManager.Paste(this));
        public ICommand ShowErrorCommand => new UiSonActionCommand((s) => _notifier.Notify(_view.StateJustification, "Error"));

        #endregion
    }
}
