// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view representing a class or struct
    /// </summary>
    public class EncapsulatingView : GroupView, IEncapsulatingView
    {
        /// <inheritdoc/>
        public virtual object? Value
        {
            get
            {
                var instance = _type.GetDefaultValue();

                if (instance != null)
                {
                    foreach (var member in Members)
                    {
                        member.Write(instance);
                    }
                }

                return instance;
            }
        }

        /// <inheritdoc/>
        public object? DisplayValue => Value;

        /// <inheritdoc/>
        public UiType UiType => UiType.Encapsulating;

        /// <inheritdoc/>
        public Type Type => _type;

        /// <inheritdoc/>
        public override ModuleState State => _state ?? base.State;
        private ModuleState? _state;

        /// <inheritdoc/>
        public override string StateJustification => _stateJustification ?? base.StateJustification;
        private string? _stateJustification;

        private readonly Type _type;

        private readonly ValueMemberInfo? _info;

        public EncapsulatingView(Type type,
                                 int displayPriority,
                                 string? name,
                                 DisplayMode displayMode,
                                 ValueMemberInfo? info,
                                 IReadWriteView[] members)
            : base(displayPriority, name, displayMode, members)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));

            foreach (var member in members)
            {
                member.PropertyChanged += OnMemberPropertyChanged;
            }

            _info = info;
        }

        private void OnMemberPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        /// <inheritdoc/>
        public override void SetValue(object? value)
        {
            if (value != null)
            {
                var valueType = value.GetType();

                if (valueType.IsAssignableTo(_type))
                {
                    _state = ModuleState.Normal;
                    _stateJustification = string.Empty;

                    foreach (var member in Members)
                    {
                        member.Read(value);
                    }
                }
            }
            else
            {
                throw new Exception("Encapsulating view cannot be set directly to null");
            }
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            SetValue(value);
            return true;
        }

        /// <inheritdoc/>
        public override void Read(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Read called on view without member info");
            }

            TrySetValueFromRead(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public override void Write(object instance)
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
