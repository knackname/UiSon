// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    public class ReferencedUiSonClass
    {
        [UiSonTag("StringTag")]
        [UiSonTextEditUi]
        public string Field_String;

        [UiSonTag("IntTag")]
        [UiSonTextEditUi]
        public int Field_Int;
    }
}
