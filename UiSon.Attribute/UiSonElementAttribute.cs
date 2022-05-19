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
        public string GroupName { get; protected set; }

        /// <summary>
        /// The file extension for json files made from this.
        /// </summary>
        public string Extension { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="extension">The file extension for json files made from this.</param>
        /// <param name="groupName">The name of the group this element belongs to.</param>
        public UiSonElementAttribute(string extension = ".json",
                                     string groupName = null)
        {
            Extension = string.IsNullOrWhiteSpace(extension) ? ".json" : extension;
            GroupName = groupName;
        }
    }
}
