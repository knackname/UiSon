﻿// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to define it's own Ui moduels.
    /// </summary>
    public class UiSonMemberElementAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// The display mode for the member's ui moduels.
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="priority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="displayMode">The display mode for the member's ui moduels.</param>
        public UiSonMemberElementAttribute(int priority = 0, string groupName = null,
                                        DisplayMode displayMode = DisplayMode.Vertial)
        {
            GroupName = groupName;
            Priority = priority;
            DisplayMode = displayMode;
        }
    }
}
