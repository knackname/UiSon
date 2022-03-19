// UiSon, by Cameron Gale 2021

using System.Windows.Input;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Decorates another <see cref="IEditorModule"/> to act as a collection entry
    /// </summary>
    public interface ICollectionEntry : IEditorModule
    {
        /// <summary>
        /// The decorated editor module
        /// </summary>
        IEditorModule Decorated { get; }

        /// <summary>
        /// Removes the entry from its parent collection
        /// </summary>
        ICommand RemoveElement { get; }
    }
}
