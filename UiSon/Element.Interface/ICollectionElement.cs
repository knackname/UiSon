// UiSon, by Cameron Gale 2021

using System.Windows.Input;
using UiSon.Attribute;

namespace UiSon.Element.Element.Interface
{
    public interface ICollectionElement
    {
        public CollectionType Type { get; }
        public DisplayMode DisplayMode { get; }
        ICommand AddElement { get; }
    }
}
