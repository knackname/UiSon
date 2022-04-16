// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a checkbox Ui
    /// in UiSon. This Ui is only effective for string, bool and bool? values
    /// or user defined types assignable by bool.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonCheckboxUiAttribute : System.Attribute, IUiSonUiAttribute
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
        /// Constructor
        /// </summary>
        /// <param name="priority">The display priority for this Ui.</param>
        /// <param name="groupName">The group this Ui belongs to</param>
        public UiSonCheckboxUiAttribute(int priority = 0, string groupName = null)
        {
            GroupName = groupName;
            Priority = priority;
        }
    }
}
