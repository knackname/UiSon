// UiSon, by Cameron Gale 2021

using System.Collections.Generic;
using System.Windows.Controls;

namespace UiSon.Element.Element.Interface
{
    public interface IElement
    {
        string Name { get; }
        int Priority { get; }
        IEnumerable<DataGridColumn> GenerateColumns(string path);
        void Write(object instance);
        void Read(object instance);
    }
}
