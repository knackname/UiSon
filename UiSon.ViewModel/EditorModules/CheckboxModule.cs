// UiSon, by Cameron Gale 2021

using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A checkbox for bool values
    /// </summary>
    public class CheckboxModule : BaseEditorModule, ICheckboxModule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CheckboxModule(ValueView<bool> view,
                              ModuleTemplateSelector templateSelector,
                              string name,
                              int priority)
            :base(view, templateSelector, name, priority)
        {
        }
    }
}
