// UiSon, by Cameron Gale 2022

using System;

namespace UiSon.Element
{
    /// <summary>
    /// Designates a class that holds a value and validates and manages the reading, writing, setting and getting of said value as any type.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// If the element's value is nullable
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Attempts to set the value to the input.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        bool SetValue(object value);

        /// <summary>
        /// Reads data from instance and set's this element's value to it
        /// </summary>
        /// <param name="instance">The instance</param>
        void Read(object instance);

        /// <summary>
        /// Writes this element's value to the instance
        /// </summary>
        /// <param name="instance">The instance</param>
        void Write(object instance);

        /// <summary>
        /// Returns the element's value as the given type
        /// </summary>
        /// <param name="type">The type to retreieve the value as</param>
        /// <returns>The element's value as the type</returns>
        object GetValueAs(Type type);
    }
}
