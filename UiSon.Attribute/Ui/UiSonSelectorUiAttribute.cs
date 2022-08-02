// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a selector Ui in UiSon.
    /// Options and identifiers of the same index will be paired, selecting an option
    /// will cause the paired identifier's value to be saved. If no identifiers are provided,
    /// the value of the option itself will be saved.
    /// 
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    public class UiSonSelectorUiAttribute : UiSonUiAttribute
    {
        /// <inheritdoc/>
        public override UiType Type => UiType.Selector;

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
        /// <param name="optionsArrayName">Array of options to select from.</param>
        /// <param name="displayPriority">The display priority of the element.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="identifiersArrayName">Array of values paired with the options array to use as the real values written.</param>
        public UiSonSelectorUiAttribute(string optionsArrayName,
                                        int displayPriority = 0,
                                        string groupName = null,
                                        string identifiersArrayName = null)
        {
            OptionsArrayName = optionsArrayName;
            GroupName = groupName;
            DisplayPriority = displayPriority;
            IdentifiersArrayName = identifiersArrayName;
        }
    }
}
