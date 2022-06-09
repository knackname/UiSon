using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    public class SingleSelector
    {
        //[UiSonElementSelectorUi(nameof(ReferencedUiSonClass), 0, null, "IntTag")]
        //public int test;

        public int[] intArray;//= new int[] { };

        public SingleSelector NestedSelector = null;
    }
}
