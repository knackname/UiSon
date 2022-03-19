// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Provides extra options for displaying enumerables in UiSon. Options will be provided to all layers of collection
    /// I.E, if placed on a List<List<string>>, both the list and the list in the first list's members will inherit the options.
    /// Must used in additon to an <see cref="IUiSonUiAttribute"/> to identify the base Ui. In the example above, this is what represents the string.
    /// UiSon only supports enumerables with one generic type. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonGenericEnumerableAttribute : System.Attribute
    {
        /// <summary>
        /// If the collection can have elements added to or removed from within UiSon
        /// </summary>
        public bool IsModifiable { get; private set; }

        /// <summary>
        /// The collection type
        /// </summary>
        public CollectionStyle CollectionType { get; private set; }

        /// <summary>
        /// The display mode
        /// </summary>
        public DisplayMode DisplayMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isModifiable">If the collection can have elements added to or removed from within UiSon</param>
        /// <param name="displayMode">The display mode</param>
        /// <param name="collectionType">The collection type</param>
        public UiSonGenericEnumerableAttribute(bool isModifiable = true,
                                        DisplayMode displayMode = DisplayMode.Vertial,
                                        CollectionStyle collectionType = CollectionStyle.Stack)
        {
            IsModifiable = isModifiable;
            CollectionType = collectionType;
            DisplayMode = displayMode;
        }
    }
}
