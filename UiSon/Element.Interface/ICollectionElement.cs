// UiSon, by Cameron Gale 2021

using System.Windows.Input;
using UiSon.Attribute;

namespace UiSon.Element.Element.Interface
{
    /// <summary>
    /// An element representing a collection
    /// </summary>
    public interface ICollectionElement
    {
        /// <summary>
        /// item type
        /// </summary>
        public CollectionType Type { get; }

        /// <summary>
        /// Type of duiplay
        /// </summary>
        public DisplayMode DisplayMode { get; }

        /// <summary>
        /// Adds a new element to the collection
        /// </summary>
        ICommand AddElement { get; }
    }
}
