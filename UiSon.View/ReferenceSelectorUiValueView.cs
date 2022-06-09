// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class ReferenceSelectorUiValueView : NPCBase, ISelectorValueView
    {
        /// <inheritdoc/>
        public object? Value
        {
            get => _reference.HasReference ? _reference.Value : _decorated.Value;
            set => TrySetValue(value);
        }

        /// <inheritdoc/>
        public virtual object? DisplayValue
        {
            get => _reference.HasReference ? _reference.DisplayValue : _decorated.DisplayValue;
            set => Value = value;
        }

        /// <inheritdoc/>
        public IEnumerable<string> Options => _decorated.Options.Concat(_reference.ElementOptions).Distinct();

        /// <inheritdoc/>
        public bool IsValueBad => !Options.Contains(DisplayValue);

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public string? Name => _decorated.Name;

        /// <inheritdoc/>
        public UiType UiType => _decorated.UiType;

        /// <inheritdoc/>
        Type? IUiValueView.Type => _decorated.Type;

        private readonly ISelectorValueView _decorated;
        private readonly ElementReferenceView _reference;
        private readonly ValueMemberInfo? _info;

        public ReferenceSelectorUiValueView(ISelectorValueView decorated,
                                            ElementReferenceView reference,
                                            ValueMemberInfo? info)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;

            _reference = reference ?? throw new ArgumentNullException(nameof(reference));
            _reference.PropertyChanged += OnReferencePropertyChanged;

            _info = info;
        }

        private void OnReferencePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ElementReferenceView.ElementOptions):
                    OnPropertyChanged(nameof(Options));
                    OnPropertyChanged(nameof(IsValueBad));
                    break;
                case nameof(ElementReferenceView.HasReference):
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(DisplayValue));
                    break;
            }

            if (_reference.HasReference)
            {
                switch (e.PropertyName)
                {
                    case nameof(ElementReferenceView.Value):
                        OnPropertyChanged(nameof(Value));
                        break;
                    case nameof(ElementReferenceView.DisplayValue):
                        OnPropertyChanged(nameof(DisplayValue));
                        OnPropertyChanged(nameof(IsValueBad));
                        break;
                }
            }
        }

        private void OnDecoratedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!_reference.HasReference)
            {
                switch (e.PropertyName)
                {
                    case nameof(IUiValueView.Value):
                        OnPropertyChanged(nameof(Value));
                        break;
                    case nameof(IUiValueView.DisplayValue):
                        OnPropertyChanged(nameof(DisplayValue));
                        OnPropertyChanged(nameof(IsValueBad));
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            var strValue = value?.ToString() ?? "null";

            if (_reference.TrySetElementFromValue(strValue))
            {
                OnPropertyChanged(nameof(Value));
                return true;
            }
            else if (_decorated.TrySetValueFromRead(strValue))
            {
                _reference.ClearReference();
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            var strValue = value?.ToString() ?? "null";

            if (_reference.TrySetElementFromName(strValue))
            {
                OnPropertyChanged(nameof(Value));
                return true;
            }
            else if (_decorated.TrySetValue(strValue))
            {
                _reference.ClearReference();
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Read(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Read called on view without member info");
            }

            TrySetValue(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Write called on view without member info");
            }

            // the reference and decorated could be returning different types for their value
            if (Value.TryCast(_info.GetUnderlyingType(), out object cast))
            {
                _info.SetValue(instance, cast);
            }
        }
    }
}
