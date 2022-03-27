﻿// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    /// <summary>
    /// Designates the member to be represented by a selector Ui in UiSon. 
    /// Selector options will be from the enum's defined values. If the enum is nullable 'null' will be included as an option.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonEnumSelectorUiAttribute : System.Attribute, IUiSonUiAttribute
    {
        /// <summary>
        /// The name of the group this Ui belongs to
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// The display priority of this Ui
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The enum's type
        /// </summary>
        public Type EnumType { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumType">The enum the selector's options are taken from</param>
        /// <param name="priority">The display priority of this Ui</param>
        /// <param name="groupName">The name of the group this Ui belongs to</param>
        public UiSonEnumSelectorUiAttribute(Type enumType, int priority = 0, string groupName = null)
        {
            EnumType = enumType;
            GroupName = groupName;
            Priority = priority;
        }
    }
}