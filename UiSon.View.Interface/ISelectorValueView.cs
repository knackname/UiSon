// UiSon, by Cameron Gale 2022

namespace UiSon.View.Interface
{
    public interface ISelectorValueView : IUiValueView
    {
        public IEnumerable<string> Options { get; }
    }
}
