// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    public class EnumSelectors
    {        
        [UiSonEnumSelectorUi(null)]
        public int EnumSelector_int_null;

        [UiSonEnumSelectorUi(typeof(int))]
        public int EnumSelector_int_int;

        [UiSonEnumSelectorUi(typeof(TestEnum))]
        public int EnumSelector_int_testEnum;

        [UiSonEnumSelectorUi(typeof(TestEnum))]
        public int? EnumSelector_NullableInt;

        [UiSonEnumSelectorUi(typeof(TestEnum))]
        public TestEnum EnumSelector_TestEnum;

        [UiSonEnumSelectorUi(typeof(TestEnum))]
        public TestEnum? EnumSelector_NullableTestEnum;

        [UiSonEnumSelectorUi(typeof(TestEnum))]
        public string EnumSelector_String;
    }
}
