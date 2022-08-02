// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view that isn't created until its value is first requested, prevents infinate loops on views of types that reference itself
    /// </summary>
    public class NullBufferValueView : NPCBase, IUiValueView
    {
        /// <inheritdoc/>
        public int DisplayPriority => _decorated?.DisplayPriority ?? _displayPriority;
        private readonly int _displayPriority;

        /// <inheritdoc/>
        public string Name => _decorated?.Name ?? _name;
        private readonly string _name;

        /// <inheritdoc/>
        public Type Type => _decorated?.Type ?? _type;
        private readonly Type _type;

        /// <inheritdoc/>
        public object? Value => _isNull ? null : Decorated.Value;

        /// <inheritdoc/>
        public object? DisplayValue => _isNull ? "null" : Decorated.DisplayValue;

        /// <inheritdoc/>
        public UiType UiType => Decorated.UiType;

        /// <summary>
        /// The buffer's decorated view
        /// </summary>
        public IUiValueView Decorated => _decorated == null ? throw new Exception("Decorated dependant property accessed before becoming non-null") : _decorated;

        /// <inheritdoc/>
        public ModuleState State => _state ?? Decorated.State;
        private ModuleState? _state = ModuleState.Special;

        /// <inheritdoc/>
        public string StateJustification => _stateJustification ?? Decorated.StateJustification;
        private string? _stateJustification = string.Empty;

        private IUiValueView? _decorated;
        private Func<IUiValueView> _makeDecorated;
        private bool _isNull = true;

        private readonly ValueMemberInfo? _info;

        public NullBufferValueView(int displayPriority, string name, Type type, ValueMemberInfo? info, Func<IUiValueView> makeDecorated)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _makeDecorated = makeDecorated ?? throw new ArgumentNullException(nameof(makeDecorated));

            _displayPriority = displayPriority;
            _name = name;
            _info = info;
        }

        private void OnDecoratedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IUiValueView.DisplayValue):
                    OnPropertyChanged(nameof(DisplayValue));
                    break;
                case nameof(IUiValueView.State):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        public void UnNull()
        {
            _isNull = false;
            _state = null;
            _stateJustification = null;

            if (_decorated == null)
            {
                _decorated = _makeDecorated.Invoke();

                // make null so it can be cleaned up, will only be called that one time.
                _makeDecorated = null;
            }

            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(DisplayValue));
        }

        /// <inheritdoc/>
        public void SetValue(object? value)
        {
            if (value == null || value.ToString() == "null")
            {
                _isNull = true;
                _state = ModuleState.Special;
                _stateJustification = string.Empty;

                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
            }
            else
            {
                UnNull();
                Decorated.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            if (value == null || value.ToString() == "null")
            {
                _isNull = true;
                _state = ModuleState.Special;
                _stateJustification = string.Empty;

                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
                return true;
            }
            else
            {
                UnNull();
                return Decorated.TrySetValue(value);
            }
        }

        /// <inheritdoc/>
        public void SetValueFromRead(object? value)
        {
            if (value == null)
            {
                _isNull = true;
                _state = ModuleState.Special;
                _stateJustification = string.Empty;

                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));
            }
            else
            {
                UnNull();
                Decorated.SetValueFromRead(value);
            }
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            if (value == null)
            {
                _isNull = true;
                _state = ModuleState.Special;
                _stateJustification = string.Empty;

                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(DisplayValue));

                return true;
            }
            else
            {
                UnNull();
                return Decorated.TrySetValueFromRead(value);
            }
        }

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

            if (State != ModuleState.Error)
            {
                _info.SetValue(instance, Value);
            }
        }
    }
}
