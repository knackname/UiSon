// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.View.Interface
{
    public interface IGroupView : IReadWriteView
    {
        /// <summary>
        /// The group's members.
        /// </summary>
        IReadWriteView[] Members { get; }

        /// <summary>
        /// The group's display mode.
        /// </summary>
        DisplayMode DisplayMode { get; }
    }
}
