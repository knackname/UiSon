// UiSon, by Cameron Gale 2021

using System.Windows.Input;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Describes a collection member that can remove itself from its parent.
    /// </summary>
    public interface ICollectionEntryModule : IValueEditorModule
    {
        /// <summary>
        /// Removes the entry from its parent collection.
        /// </summary>
        ICommand RemoveElement { get; }

        /// <summary>
        /// If the parent collection can be modified by this entry.
        /// </summary>
        bool CanModifyCollection { get; }
    }
}
