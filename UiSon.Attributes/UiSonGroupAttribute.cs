// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UiSonGroupAttribute : UiSonAttribute
    {
        public string Name { get; private set; }
        public DisplayMode DisplayMode { get; private set; }
        public Alignment Alignment { get; private set; }
        public GroupType GroupType { get; private set; }

        public UiSonGroupAttribute(string name, int priority = 0,
                                   DisplayMode displayMode = DisplayMode.Vertial, Alignment alignment = Alignment.Stretch, GroupType groupType = GroupType.Expander)
            :base(null, priority)
        {
            Name = name;
            DisplayMode = displayMode;
            Alignment = alignment;
            GroupType = groupType;
        }
    }
}
