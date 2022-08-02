using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    [UiSonArray("MultiChoiceOptions", new object[] { 1, 2, 3 })]
    public class SingleSelector
    {
        [UiSonMultiChoiceUi("MultiChoiceOptions")]
        public List<int> poop;
    }
}
