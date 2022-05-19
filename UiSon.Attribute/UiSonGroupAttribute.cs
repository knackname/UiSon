// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Defines a group to place Ui modules into. Referenced by Ui attributes using its name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UiSonGroupAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the group this group belongs to.
        /// </summary>
        public string GroupName { get; protected set; }

        /// <summary>
        /// The display priority of the group.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The group's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The display mode for Ui modules in the group.
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The group's name.</param>
        /// <param name="priority">The display priority of the group.</param>
        /// <param name="groupName">The name of the group this group belongs to.</param>
        /// <param name="displayMode">The display mode for Ui modules in the group.</param>
        public UiSonGroupAttribute(string name,
                                   int priority = 0,
                                   string groupName = null,
                                   DisplayMode displayMode = DisplayMode.Vertial)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Priority = priority;
            DisplayMode = displayMode;
            GroupName = groupName;
        }
    }
}
