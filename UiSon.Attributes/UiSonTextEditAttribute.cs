// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    public class UiSonTextEditAttribute : UiSonRestrictedMember
    {
        public string DefaultValue { get; private set; }
        public string RegexValidation { get; private set; }

        public UiSonTextEditAttribute(string name = null, int priority = 0, string regionName = null,
                                      string defaultValue = null, string regexValidation = null)
            :base(name, regionName, priority)
        {
            DefaultValue = defaultValue;
            RegexValidation = regexValidation;
        }
    }
}
