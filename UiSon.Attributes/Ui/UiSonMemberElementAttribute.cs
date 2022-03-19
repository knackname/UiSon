// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Identifies a member with it's own UiSon attributes defined.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonMemberElementAttribute : System.Attribute, IUiSonUiAttribute
    {
        /// <summary>
        /// The name of the group this Ui belongs to
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// The display priority of this Ui
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The display mode
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="priority">The display priority of this Ui</param>
        /// <param name="groupName">The name of the group this Ui belongs to</param>
        /// <param name="displayMode">The display mode</param>
        public UiSonMemberElementAttribute(int priority = 0, string groupName = null,
                                        DisplayMode displayMode = DisplayMode.Vertial)
        {
            GroupName = groupName;
            Priority = priority;
            DisplayMode = displayMode;
        }
    }
}
