using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiSon.View.Interface
{
    public interface IAssemblyView
    {
        string Path { get; }
        IEnumerable<IElementManager> ElementManagers { get; }
        IEnumerable<KeyValuePair<string, string[]>> StringArrays { get; }
    }
}
