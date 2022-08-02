// UiSon, by Cameron Gale 2021

using System;
using UiSon.Notify.Interface;
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

        /// <inheritdoc/>
        public override Type ValueType => typeof(bool);

        /// <summary>
        /// Constructor
        /// </summary>
        public CheckboxModule(IUiValueView view,
                              ModuleTemplateSelector templateSelector, ClipBoardManager clipBoardManager, INotifier notifier)
            : base(view, templateSelector, clipBoardManager, notifier)
        {
        }
    }
}
