﻿// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to define it's own Ui moduels.
    /// 
    /// Only one Ui attribute may be used per property/field.
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
        /// <param name="displayPriority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="displayMode">The display mode for the member's ui moduels.</param>
        public UiSonMemberElementAttribute(int displayPriority = 0,
                                           string groupName = null,
                                           DisplayMode displayMode = DisplayMode.Vertial)
        {
            GroupName = groupName;
            DisplayPriority = displayPriority;
            DisplayMode = displayMode;
        }
    }
}
