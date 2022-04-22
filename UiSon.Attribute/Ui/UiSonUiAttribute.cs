// UiSon, by Cameron Gale 2022

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// An attribute used to define an Ui element.
    /// Only one Ui attribute may be used per property/field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public abstract class UiSonUiAttribute : System.Attribute
    {
        /// <summary>
        /// The name of the group this Ui belongs to.
        /// </summary>
        public string GroupName { get; protected set; }

        /// <summary>
        /// The display priority of the element.
        /// </summary>
        public int Priority { get; protected set; }
    }
}
