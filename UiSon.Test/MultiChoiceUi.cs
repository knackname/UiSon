using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonGroup("Horizontal", 0, null, DisplayMode.Horizontal)]
    [UiSonArray("MultiChoiceUi_123", new object[] { 1, 2, 3 })]
    [UiSonArray("MultiChoiceUi_abc", new object[] { "A", "B", "C" })]
    public class MultiChoiceUi
    {
        [UiSonMultiChoiceUi("MultiChoiceUi_123", 0, "Horizontal")]
        public List<int> intMultiChoice;

        [UiSonMultiChoiceUi("MultiChoiceUi_abc", 0, "Horizontal")]
        public List<string> stringMultiChoice;

        // the test enum string array is defined in Selectors.cs
        [UiSonMultiChoiceUi(nameof(TestEnum), 0, "Horizontal")]
        public List<TestEnum> enumMultiChoice;
    }
}
