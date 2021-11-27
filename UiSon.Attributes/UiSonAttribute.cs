// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    public abstract class UiSonAttribute : System.Attribute
    {
        public string Name { get; private set; }
        public string RegionName { get; private set; }
        public int Priority { get; private set; }
        public UiSonAttribute(string name, string regionName, int priority)
        {
            Name = name;
            RegionName = regionName;
            Priority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class UiSonRestrictedMember : UiSonAttribute
    {
        public UiSonRestrictedMember(string name, string regionName, int priority)
            :base(name, regionName, priority)
        {
        }
    }
}
