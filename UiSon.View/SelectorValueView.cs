// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// Allows only specific values to be set
    /// </summary>
    public class SelectorValueView : DecoratingView, ISelectorValueView
    {
        /// <inheritdoc/>
        public override object? DisplayValue
        {
            get
            {
                var cleanValue = base.Value ?? "null";

                return _converter.SecondValues.Contains(cleanValue)
                    ? _converter.SecondToFirst[cleanValue]
                    : base.DisplayValue;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<string> Options => _converter.FirstValues;

        /// <inheritdoc/>
        public override ModuleState State => _state ?? base.State;
        private ModuleState? _state;

        /// <inheritdoc/>
        public override string StateJustification => _stateJustification ?? base.StateJustification;
        private string? _stateJustification;

        private readonly Map<string, object> _converter;

        public SelectorValueView(IUiValueView decorated, ValueMemberInfo? info, Map<string, object> converter)
            : base(decorated, info)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        /// <inheritdoc/>
        public override void SetValue(object? value)
        {
            var strValue = value?.ToString() ?? "null";

            if (_converter.FirstValues.Contains(strValue))
            {
                _state = null;
                _stateJustification = null;
                _decorated.SetValue(_converter.FirstToSecond[strValue]);
            }
            else
            {
                _state = ModuleState.Error;
                _stateJustification = $"{strValue} is not an exsisting option";
                _decorated.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            var strValue = value?.ToString() ?? "null";

            if (_converter.FirstValues.Contains(strValue))
            {
                _state = null;
                _stateJustification = null;

                return _decorated.TrySetValue(value);
            }

            return false;
        }

        /// <inheritdoc/>
        public override void SetValueFromRead(object? value)
        {
            if (_converter.SecondValues.Contains(value ?? "null"))
            {
                _state = null;
                _stateJustification = null;
            }
            else
            {
                _state = ModuleState.Error;
                _stateJustification = $"{value?.ToString() ?? "null"} is not an exsisting option";
            }

            _decorated.SetValueFromRead(value);
        }

        /// <inheritdoc/>
        public override bool TrySetValueFromRead(object? value)
        {
            if (_converter.SecondValues.Contains(value ?? "null"))
            {
                _state = null;
                _stateJustification = null;

                return _decorated.TrySetValueFromRead(value);
            }

            return false;
        }
    }
}
