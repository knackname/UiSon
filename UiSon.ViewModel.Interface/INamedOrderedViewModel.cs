// UiSon, by Cameron Gale 2022

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Describes a moduel with a name and dirplay priority.
    /// </summary>
    public interface INamedOrderedViewModel
    {
        /// <summary>
        /// Display name for the moduel.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The moduel's display priority.
        /// </summary>
        public int DisplayPriority { get; }

        /// <summary>
        /// Whether or not the moduel's name is visible.
        /// </summary>
        public bool IsNameVisible { get; }
    }
}
