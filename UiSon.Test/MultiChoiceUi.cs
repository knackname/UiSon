using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonGroup("Horizontal", 0, DisplayMode.Horizontal)]
    [UiSonStringArray("MultiChoiceUi_123", new string[] { "1", "2", "3" })]
    [UiSonStringArray("MultiChoiceUi_abc", new string[] { "A", "B", "C" })]
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
