// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonStringArray(null, new string[] { })]
    [UiSonStringArray("Selectors_empty", new string[] { })]
    [UiSonStringArray("Selectors_abc", new string[] { "Apple", "Banana", "Cat" })]
    [UiSonStringArray("Selectors_num", new string[] { "2", "3", "4" })]
    [UiSonStringArray("Selectors_num", new string[] { "1337", "69", "420" })]
    [UiSonStringArray("Selectors_abc_&_num", new string[] { "Apple", "Banana", "Cat", "1337", "69", "420" })]
    [UiSonStringArray("BadEnumType", typeof(int), null)]
    [UiSonStringArray("TestEnum_Strings", typeof(TestEnum), typeof(string))]
    [UiSonStringArray("TestEnum_Nullable_Strings", typeof(TestEnum?), typeof(string))]
    [UiSonStringArray("TestEnum_Ints", typeof(TestEnum), typeof(int))]
    [UiSonStringArray("TestEnum_Nullable_Ints", typeof(TestEnum?), typeof(int))]
    [UiSonStringArray(null, null, null)]
    public class Selectors
    {
        [UiSonSelectorUi(null)]
        public string String_Selector_NullOptions;

        [UiSonSelectorUi("Undefined")]
        public string String_Selector_Undefined;

        [UiSonSelectorUi("Selectors_empty")]
        public string String_Selector_NoOptions;

        [UiSonSelectorUi("Selectors_abc")]
        public string String_Selector_stringOptions;

        [UiSonSelectorUi("Selectors_num")]
        public string String_Selector_intOptions;

        [UiSonSelectorUi("Selectors_empty")]
        public int Int_Selector_NoOptions;

        [UiSonSelectorUi("Selectors_abc")]
        public int Int_Selector_stringOptions;

        [UiSonSelectorUi("Selectors_num")]
        public int Int_Selector_intOptions;

        [UiSonSelectorUi("Selectors_abc_&_num")]
        public int Int_Selector_mixedOptions;

        //enum selectors
        [UiSonSelectorUi("BadEnumType")]
        public int EnumSelector_int_int;

        [UiSonSelectorUi("TestEnum_Strings", 0, null, "TestEnum_Ints")]
        public int EnumSelector_int_testEnum;

        [UiSonSelectorUi("TestEnum_Nullable_Strings", 0, null, "TestEnum_Nullable_Ints")]
        public int? EnumSelector_NullableInt;

        [UiSonSelectorUi("TestEnum_Strings")]
        public TestEnum EnumSelector_TestEnum;

        [UiSonSelectorUi("TestEnum_Strings")]
        public TestEnum? EnumSelector_NullableTestEnum;

        [UiSonSelectorUi("TestEnum_Strings")]
        public string EnumSelector_String;
    }
}
