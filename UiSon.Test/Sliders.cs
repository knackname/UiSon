// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonGroup("Kool Kids", 0, DisplayMode.Horizontal)]
    public class Sliders
    {
        [UiSonSliderUi(0d, 100d, 0)]
        public string zero_to_hundered_zero;

        [UiSonSliderUi(0d, 100d, 1)]
        public string zero_to_hundered_one;

        [UiSonSliderUi(-1000d, 100d, 2)]
        public string nTHou_to_hundered_two;

        [UiSonSliderUi(0d, 100d, 3, 0, "Kool Kids", true)]
        public string zero_to_hundered_three;

        [UiSonSliderUi(0d, 100d, 4, 0, "Kool Kids", true)]
        public string zero_to_hundered_four;

        [UiSonSliderUi(0d, 100d, 5, 0, "Kool Kids", true)]
        public string zero_to_hundered_five;
    }
}
