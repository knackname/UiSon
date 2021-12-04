// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    public class UiSonMemberElementAttribute : UiSonRestrictedMember
    {
        public UiSonMemberElementAttribute(int order = 0, string regionName = null)
            :base(regionName, order)
        {
        }
    }
}
