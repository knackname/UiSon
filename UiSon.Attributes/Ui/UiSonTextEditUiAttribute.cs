// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a textedit Ui in UiSon.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonTextEditUiAttribute : System.Attribute, IUiSonUiAttribute
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
        /// Validation regex string
        /// </summary>
        public string RegexValidation { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="priority">The name of the group this Ui belongs to</param>
        /// <param name="groupName">The display priority of this Ui</param>
        /// <param name="regexValidation">Validation regex string</param>
        public UiSonTextEditUiAttribute(int priority = 0, string groupName = null, string regexValidation = null)
        {
            GroupName = groupName;
            Priority = priority;

            RegexValidation = regexValidation;
        }
    }
}
