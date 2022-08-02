// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a multi choice Ui in UiSon.
    /// Use this attribute on a <see cref="ICollection{T}"/>, not compatable with <see cref="UiSonCollectionAttribute"/>.
    /// The user will be presented with all options and be able to select any combination of them.
    /// The selected options will be saved to the collection.
    /// 
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    public class UiSonMultiChoiceUiAttribute : UiSonUiAttribute
    {
        /// <inheritdoc/>
        public override UiType Type => UiType.Encapsulating;

        /// <summary>
        /// Options to select from.
        /// </summary>
        public string OptionsArrayName { get; private set; }

        /// <summary>
        /// The display mode for the options.
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor with options array.
        /// </summary>
        /// <param name="optionsArrayName">Name of array with options to select from.</param>
        /// <param name="displayPriority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="displayMode">The display mode for the options.</param>
        public UiSonMultiChoiceUiAttribute(string optionsArrayName,
                                           int displayPriority = 0,
                                           string groupName = null,
                                           DisplayMode displayMode = DisplayMode.Wrap)
        {
            OptionsArrayName = optionsArrayName;
            GroupName = groupName;
            DisplayPriority = displayPriority;
            DisplayMode = displayMode;
        }
    }
}
