// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Dummy moduel, returned in the case of a failed ui module creation
    /// </summary>
    public class TextBlockModule : BaseEditorModule, ITextBlockModule
    {
        public override ModuleState State => _state;
        private readonly ModuleState _state;

        /// <summary>
        /// constructor
        /// </summary>
        public TextBlockModule(IReadWriteView view,
                               ModuleTemplateSelector templateSelector,
                               int displayPriority,
                               ModuleState state)
            :base(view, templateSelector, null, displayPriority)
        {
            _state = state;
        }
    }
}
