// UiSon, by Cameron Gale 2021

using System.Windows.Input;

namespace UiSon.Element.Element.Interface
{
    interface ICollectionEntry
    {
        IElement Element { get; }
        ICommand RemoveElement { get; }
    }
}
