// UiSon, by Cameron Gale 2022

using UiSon.Attribute;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view with a static value, will not read or write.
    /// </summary>
    public class StaticView : NPCBase, IUiValueView
    {
        /// <inheritdoc/>
        public object? Value => _value;
        private object? _value;

        /// <inheritdoc/>
        public object? DisplayValue => Value;

        /// <inheritdoc/>
        public bool IsValueBad { get; private set; }

        /// <inheritdoc/>
        public int DisplayPriority { get; private set; }

        /// <inheritdoc/>
        public string? Name => null;

        /// <inheritdoc/>
        public UiType UiType => UiType.Label;

        /// <inheritdoc/>
        public Type? Type => null;

        public StaticView(object? value, bool isValueBad, int displayPriority)
        {
            _value = value;
            IsValueBad = isValueBad;
            DisplayPriority = displayPriority;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value) => true;

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? instance) => true;

        /// <inheritdoc/>
        public void Read(object instance)
        {
            // no op
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            // no op
        }
    }
}
