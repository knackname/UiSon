// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    public class UiSonSelectorAttribute : UiSonRestrictedMember
    {
        public string DefaultValue { get; private set; }

        public string[] Options { get; private set; }

        public UiSonSelectorAttribute(string[] options,
                                      int priority = 0, string regionName = null, 
                                      string defaultValue = null)
            : base(regionName, priority)
        {
            DefaultValue = defaultValue;
            Options = options;
        }
    }
}
