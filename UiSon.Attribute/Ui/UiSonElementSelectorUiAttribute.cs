// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a selector Ui in UiSon.
    /// Selector options will be from the elements created in UiSon of the designated elementName along with any additional
    /// options defined in this attribute. In the case of an element sharing a name with an additional option,
    /// the element will take priority.
    /// A <see cref="UiSonTagAttribute"/> can be defined on a property/field of the designated UiSonElement to select an identifier.
    /// The saved value will then be the value of the identifier. If no identifierTagName is provided, the saved value will be
    /// the name of the selected element.
    /// 
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    public class UiSonElementSelectorUiAttribute : UiSonSelectorUiAttribute
    {
        /// <summary>
        /// The name of the UiSon object being selected.
        /// </summary>
        public string ElementName { get; private set; }

        /// <summary>
        /// The name of the tag attribute on the identifier.
        /// </summary>
        public string IdentifierTagName { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementName">The name of the element being selected.</param>
        /// <param name="displayPriority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="identifierTagName">The name of the tag attribute on the identifier.</param>
        /// <param name="additionalOptionsArrayName">Additional options to be selected.</param>
        /// <param name="additionalOptionsIdentifiersArrayName">Identifiers to be mapped with the additional options.</param>
        public UiSonElementSelectorUiAttribute(string elementName,
                                               int displayPriority = 0,
                                               string groupName = null,
                                               string identifierTagName = null,
                                               string additionalOptionsArrayName = null,
                                               string additionalOptionsIdentifiersArrayName = null)
            : base(additionalOptionsArrayName, displayPriority, groupName, additionalOptionsIdentifiersArrayName)
        {
            ElementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
            IdentifierTagName = identifierTagName;
        }
    }
}
