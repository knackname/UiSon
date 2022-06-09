// UiSon, by Cameron Gale 2022

namespace UiSon.View.Interface
{
    public interface IRangeView : IUiValueView
    {
        double Min { get; }
        double Max { get; }
        int Percision { get; }
        bool IsVertical { get; }
    }
}
