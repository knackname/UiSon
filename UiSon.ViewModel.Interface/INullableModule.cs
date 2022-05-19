// UiSon, by Cameron Gale 2022

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// For identification
    /// </summary>
    public interface INullableModule : IEditorModule
    {
        /// <summary>
        /// If the collapsable is expanded
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// If the value is null
        /// </summary>
        bool IsNull { get; set; }

        /// <summary>
        /// The decorated module
        /// </summary>
        IEditorModule Decorated { get; }
    }
}
