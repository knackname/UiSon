// UiSon, by Cameron Gale 2022

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A module displayed as a slider
    /// </summary>
    public interface ISliderModule : IEditorModule
    {
        /// <summary>
        /// If the slider should be vertical. Will be horizontal otherwise.
        /// </summary>
        bool IsVertical { get; }

        /// <summary>
        /// The minimum value of the slider
        /// </summary>
        double Min { get; }

        /// <summary>
        /// The maximum value of the slider
        /// </summary>
        double Max { get; }
    }
}