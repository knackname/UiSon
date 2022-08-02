// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Element;

namespace UiSon.View.Interface
{
    public interface IReadWriteView : INotifyPropertyChanged
    {
        /// <summary>
        /// The view's display priority.
        /// </summary>
        int DisplayPriority { get; }

        /// <summary>
        /// The name.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Sets the view's value to the input.
        /// </summary>
        /// <param name="value">input value</param>
        void SetValue(object? value);

        /// <summary>
        /// Attempts to set the view's value to the input. The value will not be set if
        /// it would cause an error state.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        bool TrySetValue(object? value);

        /// <summary>
        /// Set the view's value from an input that was read from a file.
        /// </summary>
        /// <param name="value">input value</param>
        void SetValueFromRead(object? value);

        /// <summary>
        /// Set the view's value from an input that was read from a file. The value will not be
        /// set if it would cause an error state.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        bool TrySetValueFromRead(object? value);

        /// <summary>
        /// Reads data from an instance and set's this view's value to it
        /// </summary>
        /// <param name="instance">The instance</param>
        void Read(object instance);

        /// <summary>
        /// Writes this view's value to the instance
        /// </summary>
        /// <param name="instance">The instance</param>
        void Write(object instance);

        /// <summary>
        /// The view's current state.
        /// </summary>
        ModuleState State { get; }

        /// <summary>
        /// The reason for the current state.
        /// </summary>
        string StateJustification { get; }
    }
}
