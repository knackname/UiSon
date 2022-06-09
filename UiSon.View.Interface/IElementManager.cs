
using System.ComponentModel;

namespace UiSon.View.Interface
{
    public interface IElementManager : INotifyPropertyChanged
    {
        string ElementName { get; }
        string Extension { get; }
        IEnumerable<IElementView> Elements { get; }
        IElementView NewElement(string name);
        Type ElementType { get; }
        void Save(string path);
    }
}
