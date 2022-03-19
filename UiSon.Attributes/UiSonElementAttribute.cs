// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Identifies a class or struct so Json files representing an instance will be creatable in UiSon
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class UiSonElementAttribute : System.Attribute
    {
        /// <summary>
        /// A json string representing the instance used for new elements of this type
        /// </summary>
        public string InitialValue { get; private set; }

        /// <summary>
        /// The file extension for json files made from this
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// If jsons generated should include fields.
        /// </summary>
        public bool IncludeFields;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="includeFields">If jsons generated should include fields.</param>
        /// <param name="extension">The file extension for json files made from this</param>
        /// <param name="initialValue">A json string representing the instance used for new elements of this type</param>
        public UiSonElementAttribute(bool includeFields = true, string extension = ".json", string initialValue = null)
        {
            IncludeFields = includeFields;
            InitialValue = initialValue;
            Extension = extension;
        }
    }
}
