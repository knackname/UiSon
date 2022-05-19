// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a textedit Ui in UiSon.
    /// The input string will be parsed to the represented property/field's type.
    /// Includes an optional string 'regexValidation' paramiter which will validate
    /// all input and only allow those satisfying the regex.
    /// 
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    public class UiSonTextEditUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Validation regex string.
        /// </summary>
        public string RegexValidation { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="displayPriority">The name of the group this Ui belongs to.</param>
        /// <param name="groupName">The display priority of this Ui.</param>
        /// <param name="regexValidation">Validation regex string.</param>
        public UiSonTextEditUiAttribute(int displayPriority = 0,
                                        string groupName = null,
                                        string regexValidation = null)
        {
            GroupName = groupName;
            DisplayPriority = displayPriority;
            RegexValidation = regexValidation;
        }
    }
}
