﻿// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the property/field to be represented by a multi choice Ui in UiSon.
    /// Use this attribute on a <see cref="ICollection{T}"/>, not compatable with <see cref="UiSonCollectionAttribute"/>.
    /// The user will be presented with all options and be able to select any combination of them.
    /// The selected options will be saved to the collection.
    /// </summary>
    public class UiSonMultiChoiceUiAttribute : UiSonUiAttribute
    {
        /// <summary>
        /// Options to select from.
        /// </summary>
        public string[] Options { get; private set; }

        /// <summary>
        /// Enum type to generate options from.
        /// </summary>
        public Type EnumType { get; private set; }

        /// <summary>
        /// The display mode for the options.
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor with options array.
        /// </summary>
        /// <param name="options">Options to select from.</param>
        /// <param name="priority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="displayMode">The display mode for the options.</param>
        public UiSonMultiChoiceUiAttribute(string[] options,
                                           int priority = 0, string groupName = null,
                                           DisplayMode displayMode = DisplayMode.Wrap)
        {
            Options = options;
            GroupName = groupName;
            Priority = priority;
            DisplayMode = displayMode;
        }

        /// <summary>
        /// Constructor with options taken from an enum.
        /// </summary>
        /// <param name="enumType">Enum type to generate options to select from.</param>
        /// <param name="priority">The display priority of this Ui.</param>
        /// <param name="groupName">The name of the group this Ui belongs to.</param>
        /// <param name="displayMode">The display mode for the options.</param>
        public UiSonMultiChoiceUiAttribute(Type enumType,
                                           int priority = 0, string groupName = null,
                                           DisplayMode displayMode = DisplayMode.Wrap)
        {
            EnumType = enumType;
            GroupName = groupName;
            Priority = priority;
            DisplayMode = displayMode;
        }
    }
}