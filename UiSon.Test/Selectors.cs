// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    public class Selectors
    {
        [UiSonSelectorUi(null)]
        public string String_Selector_NullOptions;

        [UiSonSelectorUi(new string[] { })]
        public string String_Selector_NoOptions;

        [UiSonSelectorUi(new string[] { "Apple", "Banana", "Cat" })]
        public string String_Selector_stringOptions;

        [UiSonSelectorUi(new string[] { "1337", "69", "420" })]
        public string String_Selector_intOptions;

        [UiSonSelectorUi(null)]
        public int Int_Selector_NullOptions;

        [UiSonSelectorUi(new string[] { })]
        public int Int_Selector_NoOptions;

        [UiSonSelectorUi(new string[] { "Apple", "Banana", "Cat" })]
        public int Int_Selector_stringOptions;

        [UiSonSelectorUi(new string[] { "1337", "69", "420" })]
        public int Int_Selector_intOptions;

        [UiSonSelectorUi(new string[] { "Apple", "Banana", "Cat", "1337", "69", "420" })]
        public int Int_Selector_mixedOptions;
    }
}
