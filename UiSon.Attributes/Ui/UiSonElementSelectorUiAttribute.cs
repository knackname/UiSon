// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a selector Ui in UiSon.
    /// Selector options will be from the elements created in UiSon of the designated elementName along with any additional
    /// options defined in this attribute. In the case of a selected element sharing a name with an additional option,
    /// the element will take priority.
    /// A <see cref="UiSonTagAttribute"/> can be defined on a member the designated UiSonElement to select an identifier. The saved value will then
    /// be the value of the identifier. If no identifing tag is provided, the saved value will be the name of the selected element.
    /// </summary>
    public class UiSonElementSelectorUiAttribute : UiSonSelectorUiAttribute
    {
        /// <summary>
        /// The name of the UiSon object being selected
        /// </summary>
        public string ElementName { get; private set; }

        /// <summary>
        /// Name of tag on designated type 
        /// </summary>
        public string IdentifierTagName { get; private set; }

        /// <summary>
        /// Constructor with string identifiers
        /// </summary>
        /// <param name="elementName">The name of the element being selected</param>
        /// <param name="priority">The display priority of this Ui</param>
        /// <param name="groupName">The name of the group this Ui belongs to</param>
        /// <param name="identifierTagName">The name of the tag attribute on the identifier</param>
        /// <param name="additionalOptions">Additional options to be selected</param>
        /// <param name="identifiers">identifiers to be paired with the additional options</param>
        public UiSonElementSelectorUiAttribute(string elementName,
                                             int priority = 0, string groupName = null,
                                             string identifierTagName = null,
                                             string[] additionalOptions = null, string[] identifiers = null)
            : base(additionalOptions, priority, groupName, identifiers)
        {
            ElementName = elementName ?? throw new ArgumentNullException(nameof(elementName));
            IdentifierTagName = identifierTagName;
        }
    }
}
