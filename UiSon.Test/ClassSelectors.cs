// UiSon, by Cameron Gale 2022


using UiSon.Attribute;

namespace UiSon.Test
{
    public class ClassSelectors
    {
        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, null)]
        public int ReferencedClassSelector_int_noTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "BadTag")]
        public int ReferencedClassSelector_int_badTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "IntTag")]
        public int ReferencedClassSelector_int_IntTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "StringTag")]
        public int ReferencedClassSelector_int_StringTag;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, null,
            new string[] { "additional_50", "additional_101", "additional_102" },
            new string[] { "50", "101", "102" })]
        public int ReferencedClassSelector_int_noTag_additionalOptions;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "BadTag",
            new string[] { "additional_50", "additional_101", "additional_102" },
            new string[] { "50", "101", "102" })]
        public int ReferencedClassSelector_int_BadTag_additionalOptions;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "IntTag",
            new string[] { "additional_50", "additional_101", "additional_102" },
            new string[] { "50", "101", "102" })]
        public int ReferencedClassSelector_int_IntTag_additionalOptions;

        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "StringTag",
            new string[] { "additional_50", "additional_101", "additional_102" },
            new string[] { "Big", "Bad", "Wolf" })]
        public int ReferencedClassSelector_int_StringTag_additionalOptions;
    }
}
