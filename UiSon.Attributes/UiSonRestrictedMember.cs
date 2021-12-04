using System;

namespace UiSon.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class UiSonRestrictedMember : UiSonAttribute
    {
        public UiSonRestrictedMember(string regionName, int priority)
            : base(regionName, priority)
        {
        }
    }
}
