// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a selector Ui in UiSon.
    /// Options and identifiers of the same index will be paired, selecting an option
    /// will cause the paired identifier's value to be saved. If no identifiers are provided,
    /// the value of the option itself will be saved.
    /// </summary>
    public class UiSonSelectorUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Options to select from. 
        /// </summary>
        public string[] Options { get; private set; }

        /// <summary>
        /// String identifiers to be paired with the additional options.
        /// </summary>
        public string[] Identifiers { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Options to select from.</param>
        /// <param name="priority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="identifiers">Identifiers to be paired with the Options.</param>
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
