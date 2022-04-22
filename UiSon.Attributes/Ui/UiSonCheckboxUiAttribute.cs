// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a checkbox Ui in UiSon.
    /// This Ui is only effective for string, bool and bool? values.
    /// </summary>
    public class UiSonCheckboxUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="priority">The display priority for this Ui.</param>
        /// <param name="groupName">The group this Ui belongs to.</param>
        public UiSonCheckboxUiAttribute(int priority = 0, string groupName = null)
        {
            GroupName = groupName;
            Priority = priority;
        }
    }
}
