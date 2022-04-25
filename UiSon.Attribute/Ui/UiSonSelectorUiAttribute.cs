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
        public string OptionsArrayName { get; private set; }

        /// <summary>
        /// String identifiers to be paired with the additional options.
        /// </summary>
        public string IdentifiersArrayName { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">Options to select from.</param>
        /// <param name="priority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="identifiers">Identifiers to be paired with the Options.</param>
        public UiSonSelectorUiAttribute(string optionsArrayName,
                                        int priority = 0, string groupName = null,
                                        string identifiersArrayName = null)
        {
            OptionsArrayName = optionsArrayName;
            GroupName = groupName;
            Priority = priority;
            IdentifiersArrayName = identifiersArrayName;
        }
    }
}
