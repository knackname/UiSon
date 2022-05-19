// UiSon, by Cameron Gale 2022

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Defines a text block to display along with the other groups and Ui.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UiSonTextBlockAttribute : System.Attribute
    {
        public string Text { get; private set; }

        public int Priority { get; private set; }

        public string GroupName { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="priority">The display priority of the group.</param>
        /// <param name="groupName">The name of the group this belongs to.</param>
        public UiSonTextBlockAttribute(string text, int priority = 0, string groupName = "")
        {
            Text = text;
            Priority = priority;
            GroupName = groupName;
        }
    }
}
