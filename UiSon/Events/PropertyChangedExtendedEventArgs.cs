// UiSon, by Cameron Gale 2021

using System.ComponentModel;

namespace UiSon.Events
{
    /// <summary>
    /// Property changes event with old and new values for comparison
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        public virtual T OldValue { get; private set; }
        public virtual T NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, T oldValue, T newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
