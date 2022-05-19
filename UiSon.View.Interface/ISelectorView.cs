// UiSon, by Cameron Gale 2022

namespace UiSon.View.Interface
{
    public interface ISelectorView : IReadWriteView
    {
        public IEnumerable<string> Options { get; }
    }
}
