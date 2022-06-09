using System.ComponentModel;

namespace UiSon.View.Interface
{
    public interface IElementView : INotifyPropertyChanged
    {
        string Name { get; set; }
        object? Value { get; }
        void Save(string path);
        Type ElementType { get; }
        IReadOnlyDictionary<string, IUiValueView> TagNameToView { get; }
        IUiValueView MainView { get; }
    }
}
