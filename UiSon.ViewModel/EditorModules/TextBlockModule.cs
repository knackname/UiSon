// UiSon, by Cameron Gale 2022

using System;
using UiSon.Notify.Interface;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Uneditable text display
    /// </summary>
    public class TextBlockModule : BaseEditorModule, ITextBlockModule
    {
        /// <inheritdoc/>
        public override Type ValueType => typeof(string);

        /// <summary>
        /// constructor
        /// </summary>
        public TextBlockModule(IUiValueView view,
                               ModuleTemplateSelector templateSelector,
                               ClipBoardManager clipBoardManager,
                               INotifier notifier)
            : base(view, templateSelector, clipBoardManager, notifier)
        {
        }
    }
}
