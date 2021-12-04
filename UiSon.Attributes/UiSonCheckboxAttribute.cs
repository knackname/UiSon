// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    public class UiSonCheckboxAttribute : UiSonRestrictedMember
    {
        public bool DefaultValue { get; private set; }

        /// <summary>
        /// Creates a checkbox in the Ui to represent the member
        /// </summary>
        /// <param name="groupName">The group this element belongs to</param>
        /// <param name="priority">The sorting priority for this eleemnt</param>
        /// <param name="defaultValue">The default value</param>
        public UiSonCheckboxAttribute(string groupName = null, int priority = 0,
                                      bool defaultValue = false)
            :base(groupName, priority)
        {
            DefaultValue = defaultValue;
        }
    }
}
