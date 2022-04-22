﻿// UiSon, by Cameron Gale 2022

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a slider Ui in UiSon.
    /// </summary>
    public class UiSonSliderUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Minimum value of the slider.
        /// </summary>
        public double Min { get; private set; }

        /// <summary>
        /// Maximum value of the slider.
        /// </summary>
        public double Max { get; private set; }

        /// <summary>
        /// Number of digits after the decimal when rounding value.
        /// </summary>
        public int Precision { get; private set; }

        /// <summary>
        /// If the slider is vertical.
        /// </summary>
        public bool IsVertical { get; private set; }

        /// <summary>
        /// Constructor with decimal range.
        /// </summary>
        /// <param name="min">Minimum value of the slider.</param>
        /// <param name="max">Maximum value of the slider.</param>
        /// <param name="precision">Number of digits after the decimal when rounding value.</param>
        /// <param name="priority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="unit">Unit string to be displayed along with the value.</param>
        /// <param name="isVertical">If the slider is vertical.</param>
        public UiSonSliderUiAttribute(double min, double max, int precision,
                                      int priority = 0, string groupName = null,
                                      bool isVertical = false)
        {
            Max = max;
            Min = min;
            Precision = precision;
            Priority = priority;
            GroupName = groupName;
            IsVertical = isVertical;
        }
    }
}