// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Identifies a class or struct so Json files representing an instance will be creatable in UiSon.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class UiSonElementAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the group this element belongs to.
        /// </summary>
        public bool AutoGenerateMemberAttributes { get; protected set; }

        /// <summary>
        /// The file extension for json files made from this.
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="extension">The file extension for json files made from this.</param>
        /// <param name="autoGenerateMemberAttributes">If UiSon should generate default attributes for those without any.</param>
        public UiSonElementAttribute(string extension = ".json",
                                     bool autoGenerateMemberAttributes = true)
        {
            Extension = string.IsNullOrWhiteSpace(extension) ? ".json" : extension;
            AutoGenerateMemberAttributes = autoGenerateMemberAttributes;
        }
    }
}
