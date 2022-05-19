// UiSon, by Cameron Gale 2022

using System.ComponentModel;

namespace UiSon.View.Interface
{
    /// <summary>
    /// Designates a class that holds a value and validates and manages the reading, writing, setting and getting of said value as any type.
    /// </summary>
    public interface IReadWriteView : INotifyPropertyChanged
    {
        /// <summary>
        /// The element's value.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// Attempts to set the value to the input.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        bool TrySetValue(object? value);

        /// <summary>
        /// Attempts to set the value from an input that was read.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        bool TrySetValueFromRead(object? instance);

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
    }
}
