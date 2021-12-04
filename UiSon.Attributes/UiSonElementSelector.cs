// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    public class UiSonElementSelectorAttribute : UiSonRestrictedMember
    {
        public string ElementName { get; private set; }

        public string[] Options { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="priority"></param>
        /// <param name="groupName"></param>
        /// <param name="options"></param>
        public UiSonElementSelectorAttribute(string elementName,
                                             int priority = 0, string groupName = null,
                                             string[] options = null)
            : base(groupName, priority)
        {
            ElementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
            Options = options;
        }
    }
}
