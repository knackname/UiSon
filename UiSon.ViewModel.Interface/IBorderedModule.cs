// UiSon, by Cameron Gale 2022

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A module using the border template
    /// </summary>
    public interface IBorderedModule : IEditorModule
    {
        /// <summary>
        /// The decorated module
        /// </summary>
        IEditorModule Decorated { get; }
    }
}
