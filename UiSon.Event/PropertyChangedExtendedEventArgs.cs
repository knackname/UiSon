// UiSon, by Cameron Gale 2021

using System.ComponentModel;

namespace UiSon.Event
{
    /// <summary>
    /// Property changed event with old and new values for comparison
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        /// <summary>
        /// The old value
        /// </summary>
        public virtual T OldValue { get; private set; }

        /// <summary>
        /// The new value
        /// </summary>
        public virtual T NewValue { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyName">The property's name</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
