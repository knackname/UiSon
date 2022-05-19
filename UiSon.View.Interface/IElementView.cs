using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiSon.View.Interface
{
    public interface IElementView
    {
        string Name { get; set; }
        object? Value { get; }
        void Save(string path);
    }
}
