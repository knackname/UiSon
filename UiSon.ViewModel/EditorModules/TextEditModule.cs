// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A text editor for string values
    /// </summary>
    public class TextEditModule : BaseEditorModule, ITextEditModule
    {
        /// <inheritdoc/>
        public override object Value
        {
            get => base.Value ?? "null";
            set => base.Value = value;
        }

        public TextEditModule(IUiValueView view,
                              ModuleTemplateSelector templateSelector)
            :base(view, templateSelector)
        {
        }
    }
}
