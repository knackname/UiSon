// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    public class UiSonMemberElementAttribute : UiSonRestrictedMember
    {
        public UiSonMemberElementAttribute(string name = null, int order = 0, string regionName = null)
            :base(name, regionName, order)
        {
        }
    }
}
