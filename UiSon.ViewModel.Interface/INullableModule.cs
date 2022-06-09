// UiSon, by Cameron Gale 2022

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A module able to be set to null and expanded 
    /// </summary>
    public interface INullableModule : IValueEditorModule
    {
        /// <summary>
        /// If the collapsable is expanded
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// If the null checkbox is checked
        /// </summary>
        bool IsNull { get; set; }

        /// <summary>
        /// The decorated module
        /// </summary>
        IValueEditorModule Decorated { get; }
    }
}
