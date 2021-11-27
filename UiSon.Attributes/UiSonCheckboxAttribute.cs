// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    public class UiSonCheckboxAttribute : UiSonRestrictedMember
    {
        public bool DefaultValue { get; private set; }

        public UiSonCheckboxAttribute(string name = null, string regionName = null, int priority = 0,
                                      bool defaultValue = false)
            :base(name, regionName, priority)
        {
            DefaultValue = defaultValue;
        }
    }
}
