using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using UiSon.Element;
using UiSon.Element.Element.Interface;
using UiSon.ViewModel;

namespace UiSon.Views
{
    public partial class ElementEditor : INotifyPropertyChanged
    {
        public ElementVM ElementVM { get; private set; }

        private TabControl _controller;

        public ElementEditor(ElementVM elementVM, TabControl controller)
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

        // Generates columns
        private void DataGrid_Initialized(object sender, EventArgs e)
        {
            var dataGrid = sender as DataGrid;

            // Because elements are dynamically constructed, collections must be inited with
            // one entry to use as a template for this step
            (dataGrid.DataContext as ICollectionElement).AddElement.Execute(null);

            var collectionEntry = (dataGrid.Items[dataGrid.Items.Count - 1] as ICollectionEntry);

            foreach (var column in collectionEntry.Element.GenerateColumns("Element"))
            {
                dataGrid.Columns.Add(column);
            }

            // remove template entry
            collectionEntry.RemoveElement.Execute(null);
        }
    }
}
