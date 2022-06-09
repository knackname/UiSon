// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// Allows only specific values to be set
    /// </summary>
    public class SelectorValueView : NPCBase, ISelectorValueView
    {
        /// <inheritdoc/>
        public IEnumerable<string> Options => _converter.FirstValues;

        /// <inheritdoc/>
        public object? Value => _decorated.Value;

        /// <inheritdoc/>
        public object? DisplayValue => IsValueBad ? null : _converter.SecondToFirst[_decorated.DisplayValue?.ToString() ?? "null"]; // Display value is not null here, IsValueBad checks

        /// <inheritdoc/>
        public bool IsValueBad => !_converter.SecondValues.Any(x => x == (_decorated.DisplayValue?.ToString() ?? "null"));

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public string? Name => _decorated.Name;

        /// <inheritdoc/>
        public UiType UiType => _decorated.UiType;

        /// <inheritdoc/>
        public Type? Type => _decorated.Type;

        private readonly IUiValueView _decorated;
        private readonly Map<string, string> _converter;
        private readonly ValueMemberInfo? _info;

        public SelectorValueView(IUiValueView decorated, Map<string, string> converter, ValueMemberInfo? info)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;

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
                case nameof(IUiValueView.IsValueBad):
                    OnPropertyChanged(nameof(IsValueBad));
                    break;
            }
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            var strValue = value?.ToString();

            if (strValue == null || strValue == "null")
            {
                if (_converter.FirstValues.Contains("null")
                    && _decorated.TrySetValue(null))
                {
                    OnPropertyChanged(nameof(Value));
                    return true;
                }
            }
            else if (strValue != null && _converter.FirstValues.Contains(strValue)
                     && _decorated.TrySetValue(_converter.FirstToSecond[strValue]))
            {
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual bool TrySetValueFromRead(object? value)
        {
            var strValue = value?.ToString();

            if (strValue == null || strValue == "null")
            {
                if (_converter.FirstValues.Contains("null")
                    && _decorated.TrySetValueFromRead(null))
                {
                    OnPropertyChanged(nameof(Value));
                    return true;
                }
            }
            else if (_converter.SecondValues.Contains(strValue)
                     && _decorated.TrySetValueFromRead(value))
            {
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
                _info.SetValue(instance, Value);
            }
        }
    }
}
