// UiSon, by Cameron Gale 2021

using System.Windows.Input;

namespace UiSon.Element.Element.Interface
{
    /// <summary>
    /// Entry in an element collection
    /// </summary>
    interface ICollectionEntry
    {
        /// <summary>
        /// The element
        /// </summary>
        IElement Element { get; }

        /// <summary>
        /// Removes the entry from its parent collection
        /// </summary>
        ICommand RemoveElement { get; }
    }
}
