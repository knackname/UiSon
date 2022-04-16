using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonGroup("Horizontal", 0, DisplayMode.Horizontal)]
    public class MultiChoiceUi
    {
        [UiSonMultiChoiceUi(new string[] { "1", "2", "3" }, 0, "Horizontal")]
        public List<int> intMultiChoice;

        [UiSonMultiChoiceUi(new string[] { "A", "B", "C" }, 0, "Horizontal")]
        public List<string> stringMultiChoice;

        [UiSonMultiChoiceUi(typeof(TestEnum), 0, "Horizontal")]
        public List<TestEnum> enumMultiChoice;
    }
}
