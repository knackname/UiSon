// UiSon, by Cameron Gale 2022

using System;
using UiSon.Notify.Interface;
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

        /// <inheritdoc/>
        public override Type ValueType => typeof(string);

        public TextEditModule(IUiValueView view,
                              ModuleTemplateSelector templateSelector,
                              ClipBoardManager clipBoardManager,
                              INotifier notifier)
            :base(view, templateSelector, clipBoardManager, notifier)
        {
        }
    }
}
