// UiSon, by Cameron Gale 2021

using System.Windows;
using System.Windows.Input;
using UiSon.Attribute;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// An element representing a collection
    /// </summary>
    public interface ICollectionVM
    {
        /// <summary>
        /// Type of display
        /// </summary>
        public DisplayMode DisplayMode { get; }

        /// <summary>
        /// Adds a new element to the collection
        /// </summary>
        ICommand AddEntry { get; }

        /// <summary>
        /// visiablity of collection modifying components.
        /// </summary>
        public Visibility ModifyVisibility { get; }
    }
}
