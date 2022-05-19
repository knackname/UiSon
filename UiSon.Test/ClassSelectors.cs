// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonArray("ClassSelectors_Numbers_Text", new object[] { "additional_50", "additional_101", "additional_102" })]
    [UiSonArray("ClassSelectors_Numbers_Values", new object[] { 50, 101, 102 })]
    [UiSonArray(ClassSelectors.poop, new object[] { "Big", "Bad", "Wolf" })]
    public class ClassSelectors
    {
        const string poop = "poop";

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, null)]
        public int ReferencedClassSelector_int_noTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "BadTag")]
        public int ReferencedClassSelector_int_badTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "IntTag")]
        public int ReferencedClassSelector_int_IntTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "StringTag")]
        public int ReferencedClassSelector_int_StringTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, null, "ClassSelectors_Numbers_Text", "ClassSelectors_Numbers_Values")]
        public int ReferencedClassSelector_int_noTag_additionalOptions;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "BadTag", "ClassSelectors_Numbers_Text", "ClassSelectors_Numbers_Values")]
        public int ReferencedClassSelector_int_BadTag_additionalOptions;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "IntTag", "ClassSelectors_Numbers_Text", "ClassSelectors_Numbers_Values")]
        public int ReferencedClassSelector_int_IntTag_additionalOptions;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "StringTag", "ClassSelectors_Numbers_Text", poop)]
        public int ReferencedClassSelector_int_StringTag_additionalOptions;
    }
}
