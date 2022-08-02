using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    [UiSonArray("names", new object[] { "A", "B", "C" })]
    [UiSonArray("ids", new object[] { 1, 2, 3 })]
    public class EleEnumSelectorTest
    {
        [UiSonElementSelectorUi(nameof(EleEnumSelectorRef), 0, "Items", "Id", "names", "ids")]
        public List<int?> items;
    }

    [UiSonElement]
    public class EleEnumSelectorRef
    {
        [UiSonTextEditUi]
        [UiSonTag("Id")]
        private int id;
    }
}
