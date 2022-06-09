// UiSon, by Cameron Gale 2022

using UiSon.View.Interface;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// an editor module with a value 
    /// </summary>
    public interface IValueEditorModule : IEditorModule
    {
        /// <summary>
        /// The module's value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The module's view
        /// </summary>
        IUiValueView View { get; }
    }
}
