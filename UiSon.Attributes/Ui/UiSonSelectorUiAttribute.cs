// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a selector Ui in UiSon. Options and identifiers of the same index will be pared,
    /// selecting an option will cause the pared identifier's value to be saved. If no identifiers are provided, The value of the option itself will be saved.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonSelectorUiAttribute : System.Attribute, IUiSonUiAttribute
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
        /// Additional options 
        /// </summary>
        public string[] Options { get; private set; }

        /// <summary>
        /// string identifiers to be paired with the additional options
        /// </summary>
        public string[] Identifiers { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Options to be selected</param>
        /// <param name="priority">The display priority of this Ui</param>
        /// <param name="groupName">The name of the group this Ui belongs to</param>
        /// <param name="identifiers">identifiers to be paired with the additional options</param>
        public UiSonSelectorUiAttribute(string[] options,
                                        int priority = 0, string groupName = null,
                                        string[] identifiers = null)
        {
            Options = options;

            GroupName = groupName;
            Priority = priority;

            Identifiers = identifiers;
        }
    }
}
