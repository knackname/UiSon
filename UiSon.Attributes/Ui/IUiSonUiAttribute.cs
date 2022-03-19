// UiSon, by Cameron Gale 2022

namespace UiSon.Attribute
{
    /// <summary>
    /// Identifies an attribute used to define a Ui element
    /// </summary>
    public interface IUiSonUiAttribute
    {
        /// <summary>
        /// The name of the group this Ui belongs to.
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// The display priority of the element.
        /// </summary>
        int Priority { get; }
    }
}
