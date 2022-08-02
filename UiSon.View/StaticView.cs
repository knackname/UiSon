// UiSon, by Cameron Gale 2022

using UiSon.Attribute;
using UiSon.Element;
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
        public int DisplayPriority { get; private set; }

        /// <inheritdoc/>
        public string? Name => null;

        /// <inheritdoc/>
        public UiType UiType => UiType.Label;

        /// <inheritdoc/>
        public Type? Type => null;

        /// <inheritdoc/>
        public ModuleState State => _state;
        private readonly ModuleState _state;

        /// <inheritdoc/>
        public string StateJustification => _stateJustification;
        private readonly string _stateJustification;

        public StaticView(object? value, int displayPriority, ModuleState state, string stateJustification)
        {
            _value = value;
            _state = state;
            _stateJustification = stateJustification;
            DisplayPriority = displayPriority;
        }

        /// <inheritdoc/>
        public void SetValue(object? value)
        {
            // no op
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value) => true;

        /// <inheritdoc/>
        public void SetValueFromRead(object? value)
        {
            // no op
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value) => true;

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
