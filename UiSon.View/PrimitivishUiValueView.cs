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
        private object? _value = null;

        /// <inheritdoc/>
        public object? DisplayValue => Value;

        /// <inheritdoc/>
        public int DisplayPriority { get; private set; }

        /// <inheritdoc/>
        public string? Name { get; private set; }

        /// <inheritdoc/>
        public UiType UiType { get; private set; }

        /// <inheritdoc/>
        public Type Type => _type;

        /// <inheritdoc/>
        public virtual ModuleState State => _state;
        private ModuleState _state = ModuleState.Normal;

        /// <inheritdoc/>
        public virtual string StateJustification => _stateJustification;
        private string _stateJustification = string.Empty;

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
        public void SetValue(object? value)
        {
            if (value == null || (value as string) == "null")
            {
                _value = null;

                if (_isNullable)
                {
                    _state = ModuleState.Special;
                    _stateJustification = string.Empty;
                }
                else
                {
                    _state = ModuleState.Error;
                    _stateJustification = "Value cannot be null";
                }
            }
            else if (value.TryCast(_type, out var asT))
            {
                _value = asT;
                _state = ModuleState.Normal;
                _stateJustification = string.Empty;
            }
            else
            {
                _value = value;
                _state = ModuleState.Error;
                _stateJustification = $"Object with type {value?.GetType().ToString() ?? "null"} and value {value} cannot be cast as type {_type}";
            }

            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(DisplayValue));
            OnPropertyChanged(nameof(State));
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            if (value == null || (value as string) == "null")
            {
                if (_isNullable)
                {
                    _value = null;
                    _state = ModuleState.Special;
                    _stateJustification = string.Empty;

                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(DisplayValue));
                    OnPropertyChanged(nameof(State));
                    return true;
                }
            }
            else if (value.TryCast(_type, out var asT))
            {
                _value = asT;
                _state = ModuleState.Normal;
                _stateJustification = string.Empty;

                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
                OnPropertyChanged(nameof(State));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual void SetValueFromRead(object? value) => SetValue(value);

        /// <inheritdoc/>
        public virtual bool TrySetValueFromRead(object? value) => TrySetValue(value);

        /// <inheritdoc/>
        public void Read(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Read called on view without member info");
            }

            SetValueFromRead(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Write called on view without member info");
            }

            if (_state != ModuleState.Error)
            {
                _info.SetValue(instance, Value);
            }
        }
    }
}
