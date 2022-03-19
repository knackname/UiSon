// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Defines a group to place Ui elements into. Referenced by Ui attributes using its name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UiSonGroupAttribute : System.Attribute
    {
        /// <summary>
        /// The display priority of this Ui
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The group's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The display mode
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The group's name</param>
        /// <param name="priority">The display priority of this Ui</param>
        /// <param name="displayMode">The display mode</param>
        public UiSonGroupAttribute(string name,
                                   int priority = 0,
                                   DisplayMode displayMode = DisplayMode.Vertial)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Priority = priority;
            DisplayMode = displayMode;
        }
    }
}
