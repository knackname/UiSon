
namespace UiSon.View.Interface
{
    public interface IElementManager
    {
        string ElementName { get; }
        string Extension { get; }
        IEnumerable<IElementView> Elements { get; }
        IElementView NewElement(string name, object? initialValue);
        Type ElementType { get; }
        void Save(string path);
    }
}