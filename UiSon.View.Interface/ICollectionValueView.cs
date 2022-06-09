// UiSon, by Cameron Gale 2022

namespace UiSon.View.Interface
{
    public interface ICollectionValueView : IGroupView, IUiValueView
    {
        /// <summary>
        /// The collection's entries
        /// </summary>
        IEnumerable<IUiValueView> Entries { get; }

        /// <summary>
        /// If the collection can be modified
        /// </summary>
        bool IsModifiable { get; }

        /// <summary>
        /// Adds a new entry to the collection
        /// </summary>
        IUiValueView AddEntry();

        /// <summary>
        /// Remove an entry from the collection
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        void RemoveEntry(IUiValueView entry);
    }
}
