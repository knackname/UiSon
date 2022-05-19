// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;

namespace UiSon.Attribute
{
    /// <summary>
    /// Provides extra options for displaying <see cref="ICollection{T}"/> in UiSon. Options will be provided to all layers of collection
    /// i.e, if placed on a List<List<string>>, both the list and the list in the first list's members will inherit the options.
    /// Must be used in additon to an <see cref="UiSonUiAttribute"/> to identify the base Ui. In the example above, this is what represents the string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonCollectionAttribute : System.Attribute
    {
        /// <summary>
        /// If the collection can have elements added to or removed from within UiSon.
        /// </summary>
        public bool IsModifiable { get; private set; }

        /// <summary>
        /// The display mode for collection members.
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isModifiable">If the collection can have items added to or removed from within UiSon.</param>
        /// <param name="displayMode">The display mode for collection members.</param>
        public UiSonCollectionAttribute(bool isModifiable = true,
                                        DisplayMode displayMode = DisplayMode.Vertial)
        {
            IsModifiable = isModifiable;
            DisplayMode = displayMode;
        }
    }
}
