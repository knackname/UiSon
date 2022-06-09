// UiSon, by Cameron Gale 2022

using System.ComponentModel;

namespace UiSon.View.Interface
{
    public interface IReadWriteView : INotifyPropertyChanged
    {
        /// <summary>
        /// The view's display priority.
        /// </summary>
        int DisplayPriority { get; }

        /// <summary>
        /// If the view's value is invalid.
        /// </summary>
        public bool IsValueBad { get; }

        /// <summary>
        /// The name.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Attempts to set the view's value to the input.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        bool TrySetValue(object? value);

        /// <summary>
        /// Attempts to set the view's value from an input that was read.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
    }
}
