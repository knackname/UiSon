// UiSon, by Cameron Gale 2021

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public partial class ElementEditorTab : INotifyPropertyChanged, IElementEditorTab
    {
        public string ElementName
        {
            get => _view.Name;
            set
            {
                if (_view.Name != value)
                {
                    _view.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public IEditorModule MainModule => _mainModule;
        private readonly IEditorModule _mainModule;

        public IElementView View => _view;
        private readonly IElementView _view;
        private readonly UiSonUi _parent;

        public ElementEditorTab(IElementView view, IEditorModule mainModule, UiSonUi parent)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;

            _mainModule = mainModule ?? throw new ArgumentNullException(nameof(mainModule));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            DataContext = this;

            InitializeComponent();
        }

        public void Destroy()
        {
            DataContext = null;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => _parent.CloseTab(this);

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ElementView.Name):
                    OnPropertyChanged(nameof(Name));
                    break;
            }
        }

        private void DataGridCollection_Initialized(object sender, EventArgs e)
        {
            var dataGrid = (DataGrid)sender;
            var collection = (ICollectionModule)dataGrid.DataContext;

            // Because modules are dynamically constructed, collections must be inited with
            // one entry to use as a template for this step
            collection.AddEntryCommand.Execute(dataGrid);

            var collectionEntry = (ICollectionEntryModule)dataGrid.Items[dataGrid.Items.Count - 1];

            foreach (var column in collectionEntry.GenerateColumns(string.Empty))
            {
                dataGrid.Columns.Add(column);
            }

            // remove template entry
            collectionEntry.RemoveElement.Execute(null);

            // also, template columns have quirky bindings and I don't like them, so let's set the modify vis here as well
            dataGrid.Columns[0].Visibility = collection.CanModifyCollection ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GridGroup_Initialized(object sender, EventArgs e)
        {
            var dataGrid = (DataGrid)sender;

            foreach (var column in ((IGroupModule)dataGrid.DataContext).GenerateColumns(string.Empty))
            {
                dataGrid.Columns.Add(column);
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
