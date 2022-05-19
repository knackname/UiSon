// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A value type <see cref="IUiSonElement"/>
    /// </summary>
    /// <typeparam name="T">A Value type</typeparam>
    public class ValueView<T> : NPCBase, IReadWriteView
        where T : struct
    {
        /// <inheritdoc/>
        public virtual object? Value => _value;
        protected T? _value = null;

        private readonly bool _isNullable;
        private readonly ValueMemberInfo? _info;
        private readonly Type _type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The type this writes as.</param>
        /// <param name="info">The info for write.</param>
        public ValueView(Type type, ValueMemberInfo? info)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            
            var utype = Nullable.GetUnderlyingType(type);
            _isNullable = !type.IsValueType || utype != null;
            _type = utype ?? type;

            _info = info;
        }

        /// <inheritdoc/>
        public virtual bool TrySetValue(object? input)
        {
            // null
            if (input == null)
            {
                if (_isNullable)
                {
                    _value = null;
                    OnPropertyChanged(nameof(Value));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // make sure value can be set from the element's type
            else if (input.TryCast(typeof(T), out var asT)
                     && asT.TryCast(_type, out var _))
            {
                _value = (T)asT;
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value) => TrySetValue(value);

        /// <inheritdoc/>
        public void Read(object instance) => TrySetValue(_info.GetValue(instance));

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_value.TryCast(_type, out object casted))
            {
                _info.SetValue(instance, casted);
            }
        }
    }
}
