// UiSon, by Cameron Gale 2021

using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates an element with a border
    /// </summary>
    public class BorderedModule : BaseDecoratingEditorModule, IBorderedModule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="decorated">The decorated module</param>
        public BorderedModule(IEditorModule decorated)
            : base(decorated)
        {
        }
    }
}
