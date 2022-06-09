// UiSon, by Cameron Gale 2022

using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view representing an instance of a type directly
    /// </summary>
    public class PrimitivishUiValueView: NPCBase, IUiValueView
    {
        /// <inheritdoc/>
        public object? Value => _value;
        protected object? _value = null;

        /// <inheritdoc/>
        public object? DisplayValue => Value;

        /// <inheritdoc/>
        public bool IsValueBad => (!_isNullable && _value == null) || (!Value?.TryCast(_type, out var _) ?? false);

        /// <inheritdoc/>
        public int DisplayPriority { get; private set; }

        /// <inheritdoc/>
        public string? Name { get; private set; }

        /// <inheritdoc/>
        public UiType UiType { get; private set; }

        /// <inheritdoc/>
        public Type Type => _type;
        private readonly Type _type;

        private readonly bool _isNullable;
        private readonly ValueMemberInfo? _info;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">The type this writes as.</param>
        /// <param name="info">The info for write.</param>
        public PrimitivishUiValueView(Type type, int displayPriority, string name, UiType uiType, ValueMemberInfo? info)
        {
            UiType = uiType;
            DisplayPriority = displayPriority;
            Name = name;
            _info = info;
            
            var utype = Nullable.GetUnderlyingType(type);
            _isNullable = !type.IsValueType || utype != null;
            _type = utype ?? type;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            if (value == null || (value as string) == "null")
            {
                if (_isNullable)
                {
                    _value = null;
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(DisplayValue));
                    OnPropertyChanged(nameof(IsValueBad));
                    return true;
                }
            }
            else if (value.TryCast(_type, out var asT))
            {
                _value = asT;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
                OnPropertyChanged(nameof(IsValueBad));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual bool TrySetValueFromRead(object? value) => TrySetValue(value);

        /// <inheritdoc/>
        public void Read(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Read called on view without member info");
            }

            TrySetValueFromRead(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Write called on view without member info");
            }

            if (!IsValueBad)
            {
                _info.SetValue(instance, IsValueBad ? _type.GetDefaultValue() : Value);
            }
        }
    }
}
