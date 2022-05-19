// UiSon, by Cameron Gale 2021

using System.Windows.Input;
using UiSon.Attribute;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Describes a moduel with a collection of members that can be added to.
    /// </summary>
    public interface ICollectionModule : IEditorModule
    {
        /// <summary>
        /// Type of display
        /// </summary>
        DisplayMode DisplayMode { get; }

        /// <summary>
        /// Adds a new element to the collection
        /// </summary>
        ICommand AddEntryCommand { get; }

        /// <summary>
        /// If the collection can be modified
        /// </summary>
        bool CanModifyCollection { get; }
    }
}
