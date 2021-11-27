// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    public class UiSonElementSelectorAttribute : UiSonRestrictedMember
    {
        public string ElementName { get; private set; }

        public string[] Options { get; private set; }

        public UiSonElementSelectorAttribute(string elementName,
                                             string name = null, int priority = 0, string regionName = null,
                                             string[] options = null)
            : base(name, regionName, priority)
        {
            ElementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
            Options = options;
        }
    }
}
