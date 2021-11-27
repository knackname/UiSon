// UiSon, by Cameron Gale 2021

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UiSon.Element.Element.Interface;

namespace UiSon.Element
{
    /// <summary>
    /// Base abstract element class
    /// </summary>
    public abstract class BaseElement : NPCBase, IElement
    {
        public string Name { get; private set; }
        public int Priority { get; private set; }
        public Visibility IsNameVisible => string.IsNullOrWhiteSpace(Name) ? Visibility.Collapsed : Visibility.Visible;
        public BaseElement(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
        public abstract IEnumerable<DataGridColumn> GenerateColumns(string path);
        public abstract void Write(object instance);
        public abstract void Read(object instance);
    }
}
