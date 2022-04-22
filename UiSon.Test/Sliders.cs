// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    public class Sliders
    {
        [UiSonSliderUi(0d, 100d, 0)]
        public int zero_to_hundered_zero;

        [UiSonSliderUi(0d, 100d, 1)]
        public float zero_to_hundered_one;

        [UiSonSliderUi(0d, 100d, 2)]
        public double zero_to_hundered_two;

        [UiSonSliderUi(0d, 100d, 3, 0, null, true)]
        public string zero_to_hundered_three;
    }
}
