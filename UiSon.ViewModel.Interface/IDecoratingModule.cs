// UiSon, by Cameron Gale 2022

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A module decorating another
    /// </summary>
    public interface IDecoratingModule : IEditorModule
    {
        /// <summary>
        /// The decorated module
        /// </summary>
        IEditorModule Decorated { get; }
    }
}
