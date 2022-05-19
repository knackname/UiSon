// UiSon 2022, Cameron Gale

using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// View for a class or struct
    /// </summary>
    public class EncapsulatingView : NPCBase, IReadWriteView
    {
        /// <inheritdoc/>
        public object? Value => _value;
        private object? _value;

        private readonly bool _isNullable;
        private readonly ValueMemberInfo? _info;
        private readonly Type _type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EncapsulatingView(Type type, ValueMemberInfo? info)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _info = info;

            _isNullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            if (value == null)
            {
                if (_isNullable)
                {
                    _value = null;
                    OnPropertyChanged(nameof(Value));
                    return true;
                }

                return false;
            }
            else if (value.GetType().IsAssignableTo(_type))
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? instance) => TrySetValue(instance);

        /// <inheritdoc/>
        public void Read(object instance) => TrySetValue(_info.GetValue(instance));

        /// <inheritdoc/>
        public void Write(object instance) => _info?.SetValue(instance, _value);
    }
}
