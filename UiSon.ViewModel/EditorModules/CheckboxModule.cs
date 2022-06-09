// UiSon, by Cameron Gale 2021

using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A checkbox for bool values
    /// </summary>
    public class CheckboxModule : BaseEditorModule, ICheckboxModule
    {
        /// <inheritdoc/>
        public override object Value
        {
            get => base.Value ?? false;
            set => base.Value = value;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckboxModule(IUiValueView view,
                              ModuleTemplateSelector templateSelector)
            :base(view, templateSelector)
        {
        }
    }
}
