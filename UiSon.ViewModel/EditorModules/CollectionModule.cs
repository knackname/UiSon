// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Command;
using UiSon.Element;
using UiSon.Notify.Interface;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// An expandable collection of one kind of <see cref="IEditorModule"/>
    /// </summary>
    public class CollectionModule : NPCBase, ICollectionModule
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
        public DisplayMode DisplayMode => _view.DisplayMode;

        /// <inheritdoc/>
        public string Name => _view.Name;

        /// <inheritdoc/>
        public int DisplayPriority => _view.DisplayPriority;

        /// <inheritdoc/>
        public ModuleState State => _view.State;

        /// <inheritdoc/>
        public string StateJustification => _view.StateJustification;

        /// <inheritdoc/>
        public bool CanModifyCollection => _view.IsModifiable;

        /// <inheritdoc/>
        public bool HasError => _view.State == ModuleState.Error;

        /// <inheritdoc/>
        public ObservableCollection<ICollectionEntryModule> Entries => _entries;
        private readonly ObservableCollection<ICollectionEntryModule> _entries = new ObservableCollection<ICollectionEntryModule>();

        /// <inheritdoc/>
        public IUiValueView View => _view;
        private readonly ICollectionValueView _view;

        private readonly ModuleTemplateSelector _templateSelector;
        private readonly EditorModuleFactory _factory;
        private readonly INotifier _notifier;
        private readonly ClipBoardManager _clipBoardManager;

        public CollectionModule(ICollectionValueView view,
                                EditorModuleFactory factory,
                                ModuleTemplateSelector templateSelector,
                                ClipBoardManager clipBoardManager,
                                INotifier notifier)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;

            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));

            _clipBoardManager = clipBoardManager ?? throw new ArgumentNullException(nameof(clipBoardManager));

            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            RepopulateCollection();
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ICollectionValueView.DisplayMode):
                    OnPropertyChanged(nameof(DisplayMode));
                    break;
                case nameof(ICollectionValueView.DisplayPriority):
                    OnPropertyChanged(nameof(DisplayPriority));
                    break;
                case nameof(ICollectionValueView.State):
                    OnPropertyChanged(nameof(State));
                    OnPropertyChanged(nameof(HasError));
                    break;
                case nameof(ICollectionValueView.IsModifiable):
                    OnPropertyChanged(nameof(CanModifyCollection));
                    break;
                case nameof(ICollectionValueView.Entries):
                    RepopulateCollection();
                    break;
                case nameof(IUiValueView.DisplayValue):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        private void OnEntryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ICollectionEntryModule.State):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        /// <summary>
        /// Clears entries and regenerates them from the view
        /// </summary>
        private void RepopulateCollection()
        {
            foreach (var entry in _entries)
            {
                entry.PropertyChanged -= OnEntryPropertyChanged;
            }

            _entries.Clear();

            _view.PropertyChanged -= OnViewPropertyChanged;

            foreach (var entryView in _view.Entries)
            {
                var newEntry = new CollectionEntryModule(this, _factory.MakeUiValueEditorModule(entryView));
                newEntry.PropertyChanged += OnEntryPropertyChanged;
                _entries.Add(newEntry);
            }

            _view.PropertyChanged += OnViewPropertyChanged;

            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Value));
        }

        /// <inheritdoc/>
        public void AddEntry()
        {
            _view.PropertyChanged -= OnViewPropertyChanged;

            var newEntry = new CollectionEntryModule(this, _factory.MakeUiValueEditorModule(_view.AddEntry()));
            newEntry.PropertyChanged += OnEntryPropertyChanged;
            _entries.Add(newEntry);

            _view.PropertyChanged += OnViewPropertyChanged;

            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Value));
        }

        /// <inheritdoc/>
        public void RemoveEntry(ICollectionEntryModule entry)
        {
            // unsubscribe from viewPC so we don't have to remake the whole
            // frontend for each addition/removal
            _view.PropertyChanged -= OnViewPropertyChanged;

            entry.PropertyChanged -= OnEntryPropertyChanged;
            _entries.Remove(entry);
            _view.RemoveEntry(entry.View);

            _view.PropertyChanged += OnViewPropertyChanged;

            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Value));
        }

        /// <inheritdoc/>
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

        #region Commands

        /// <inheritdoc/>
        public ICommand AddEntryCommand => new UiSonActionCommand((s) => AddEntry());

        /// <inheritdoc/>
        public ICommand CopyCommand => new UiSonActionCommand((s) => _clipBoardManager.Copy(this));

        /// <inheritdoc/>
        public ICommand PasteCommand => new UiSonActionCommand((s) => _clipBoardManager.Paste(this));

        /// <inheritdoc/>
        public ICommand ShowErrorCommand => new UiSonActionCommand((s) => _notifier.Notify(_view.StateJustification, "Error"));

        #endregion
    }
}
