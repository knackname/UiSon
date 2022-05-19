// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a checkbox Ui in UiSon.
    /// This Ui is only effective for string, bool and bool? values.
    /// 
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    public class UiSonCheckboxUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="displayPriority">The display priority for this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        public UiSonCheckboxUiAttribute(int displayPriority = 0,
                                        string groupName = null)
        {
            GroupName = groupName;
            DisplayPriority = displayPriority;
        }
    }
}
