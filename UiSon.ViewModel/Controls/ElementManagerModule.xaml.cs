using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace UiSon.ViewModel.Controls
{
    public partial class ElementManagerModule : UserControl, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
