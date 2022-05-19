// UiSon, by Cameron Gale 2022

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a label Ui in UiSon.
    /// The label is readonly and displays the <see cref="object.ToString"/> value of the property/field.
    /// 
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    public class UiSonLabelUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="displayPriority">The display priority for this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        public UiSonLabelUiAttribute(int displayPriority = 0,
                                     string groupName = null)
        {
            GroupName = groupName;
            DisplayPriority = displayPriority;
        }
    }
}
