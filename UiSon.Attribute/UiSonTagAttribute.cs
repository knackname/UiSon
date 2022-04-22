// UiSon, by Cameron Gale 2022

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Used to tag a member for lookup by other UiSon Elements. Identified by its name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonTagAttribute : System.Attribute
    {
        /// <summary>
        /// The tag's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The tag's name.</param>
        public UiSonTagAttribute(string name)
        {
            Name = name;
        }
    }
}
