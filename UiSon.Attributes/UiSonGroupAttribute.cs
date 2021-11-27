// UiSon, by Cameron Gale 2021

using System;
using UiSon.Attribute.Enums;

namespace UiSon.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UiSonGroupAttribute : UiSonAttribute
    {
        public DisplayMode DisplayMode { get; private set; }
        public Alignment Alignment { get; private set; }
        public GroupType GroupType { get; private set; }

        public UiSonGroupAttribute(string name, int order = 0,
                                   DisplayMode displayMode = DisplayMode.Vertial, Alignment alignment = Alignment.Left, GroupType groupType = GroupType.Expander)
            :base(name, null, order)
        {
            DisplayMode = displayMode;
            Alignment = alignment;
            GroupType = groupType;
        }
    }
}
