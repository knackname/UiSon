// UiSon, by Cameron Gale 2021

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public partial class ElementEditor : INotifyPropertyChanged
    {
        public IElementVM ElementVM { get; private set; }

        private TabControl _controller;

        public ElementEditor(IElementVM elementVM, TabControl controller)
        {
            ElementVM = elementVM ?? throw new ArgumentNullException(nameof(elementVM));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            DataContext = elementVM;
            InitializeComponent();
        }

        private void Close(object sender, System.Windows.RoutedEventArgs e) => _controller.Items.Remove(this);

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        private void DataGridCollection_Initialized(object sender, EventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var collectionVM = dataGrid.DataContext as ICollectionVM;

            // Because elements are dynamically constructed, collections must be inited with
            // one entry to use as a template for this step
            collectionVM.AddEntry.Execute(dataGrid);

            var collectionEntry = dataGrid.Items[dataGrid.Items.Count - 1] as ICollectionEntry;

            foreach (var column in collectionEntry.GenerateColumns(string.Empty))
            {
                dataGrid.Columns.Add(column);
            }

            // remove template entry
            collectionEntry.RemoveElement.Execute(null);

            // also, template columns have quirky bindings and I don't like them, so let's set the modify vis here as well
            dataGrid.Columns[0].Visibility = collectionVM.ModifyVisibility;
        }

        private void GridGroup_Initialized(object sender, EventArgs e)
        {
            var dataGrid = sender as DataGrid;
            var groupVM = dataGrid.DataContext as GroupVM;

            foreach (var column in groupVM.GenerateColumns(string.Empty))
            {
                dataGrid.Columns.Add(column);
            }
        }
    }
}
