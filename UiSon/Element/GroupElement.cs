// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UiSon.Attribute;
using UiSon.Element.Element.Interface;

namespace UiSon.Element
{
    /// <summary>
    /// A group of elements, not deigned to be expandable
    /// </summary>
    public class GroupElement : BaseElement
    {
        public DisplayMode DisplayMode { get; private set; }
        public IEnumerable<IElement> Members { get; private set; }
        public GroupType GroupType { get; private set; }

        public HorizontalAlignment HorizontalAlignment { get; private set; }

        public GroupElement(string name, int priority,
            IEnumerable<IElement> members, DisplayMode displayMode = DisplayMode.Vertial, Alignment alignment = Alignment.Left, GroupType type = GroupType.Basic)
            :base(name, priority)
        {
            Members = members ?? throw new ArgumentNullException(nameof(members));
            DisplayMode = displayMode;
            GroupType = type;

            switch (alignment)
            {
                case Alignment.Left:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case Alignment.Center:
                    HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case Alignment.Right:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case Alignment.Stretch:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
            }
        }

        public override IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var columns = new List<DataGridColumn>();

            var i = 0;
            foreach (var member in Members)
            {
                foreach (var col in member.GenerateColumns(path + $".Members[{i}]"))
                {
                    columns.Add(col);
                }
                i++;
            }

            return columns;
        }

        public override void Write(object instance)
        {
            foreach (var member in Members)
            {
                member.Write(instance);
            }
        }

        public override void Read(object instance)
        {
            foreach (var member in Members)
            {
                member.Read(instance);
            }
        }
    }
}
