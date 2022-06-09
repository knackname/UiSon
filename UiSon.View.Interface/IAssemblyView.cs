// UiSon, by Cameron Gale 2022

using System.ComponentModel;

namespace UiSon.View.Interface
{
    public interface IAssemblyView : INotifyPropertyChanged
    {
        string Path { get; }
        IEnumerable<IElementManager> ElementManagers { get; }
        IEnumerable<KeyValuePair<string, string[]>> StringArrays { get; }
    }
}
