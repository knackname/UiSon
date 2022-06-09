// UiSon, by Cameron Gale 2022

using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Uneditable text display
    /// </summary>
    public class TextBlockModule : BaseEditorModule, ITextBlockModule
    {
        /// <summary>
        /// constructor
        /// </summary>
        public TextBlockModule(IUiValueView view,
                               ModuleTemplateSelector templateSelector)
            :base(view, templateSelector)
        {
        }
    }
}
