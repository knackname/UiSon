using System;
using System.Collections.Generic;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a multi choice Ui in UiSon.
    /// Use this attribute on a <see cref="ICollection{T}"/>, not compatable with <see cref="UiSonCollectionAttribute"/>.
    /// Options and identifiers of the same index will be pared,
    /// selecting an option will cause the pared identifier's value to be saved.
    /// If no identifiers are provided, The value of the option itself will be saved.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonMultiChoiceUiAttribute : System.Attribute, IUiSonUiAttribute
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
        /// The display mode
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Options to be selected</param>
        /// <param name="priority">The display priority of this Ui</param>
        /// <param name="groupName">The name of the group this Ui belongs to</param>
        /// <param name="identifiers">identifiers to be paired with the additional options</param>
        public UiSonMultiChoiceUiAttribute(string[] options,
                                           int priority = 0, string groupName = null,
                                           DisplayMode displayMode = DisplayMode.Horizontal)
        {
            Options = options;

            GroupName = groupName;
            Priority = priority;
            DisplayMode = displayMode;
        }
    }
}
