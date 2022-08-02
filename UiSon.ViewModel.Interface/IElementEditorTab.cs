// UiSon, by Cameron Gale 2022

using UiSon.View.Interface;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A tab with an element view
    /// </summary>
    public interface IElementEditorTab
    {
        IElementView View { get; }
        void Destroy();
    }
}
