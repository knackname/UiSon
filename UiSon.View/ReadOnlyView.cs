// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view with a readonly value. Calls to set will be ignored.
    /// </summary>
    public class ReadOnlyView : NPCBase, IReadWriteView
    {
        /// <inheritdoc/>
        public object? Value => _value;
        private object? _value;

        private readonly ValueMemberInfo? _info;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="info"></param>
        public ReadOnlyView(object? value, ValueMemberInfo? info = null)
        {
            _value = value;
            _info = info;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value) => false;

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? instance) => false;

        /// <inheritdoc/>
        public void Read(object instance)
        {
            if (_info != null)
            {
                _value = _info.GetValue(instance);
            }
        }

        /// <inheritdoc/>
        public void Write(object instance) => _info?.SetValue(instance, _value);
    }
}
