// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class RangeUiValueView : DecoratingView, IRangeView
    {
        /// <inheritdoc/>
        public override object? DisplayValue => _displayValue ?? base.DisplayValue;
        private string? _displayValue;

        /// <summary>
        /// Minimum value
        /// </summary>
        public double Min => _min;
        private readonly double _min;

        /// <summary>
        /// Maximum value
        /// </summary>
        public double Max => _max;
        private readonly double _max;

        /// <inheritdoc/>
        public int Percision => _precision;
        private readonly int _precision;

        /// <summary>
        /// If the ui should be vertical rather than horizontal
        /// </summary>
        public bool IsVertical => _isVertical;
        private readonly bool _isVertical;

        /// <inheritdoc/>
        public override ModuleState State => _state ?? base.State;
        private ModuleState? _state;

        /// <inheritdoc/>
        public override string StateJustification => _stateJustification ?? base.StateJustification;
        private string? _stateJustification;

        private readonly string _formatString;

        public RangeUiValueView(IUiValueView decorated, ValueMemberInfo? info, double min, double max, int percision, bool isVertical)
            : base(decorated, info)
        {
            _min = min;
            _max = max;
            _precision = percision;
            _isVertical = isVertical;

            _formatString = '{' + $"0:{new string('0', Math.Max(Math.Round(Min, 0, MidpointRounding.ToNegativeInfinity).ToString().Length - (Min < 0 ? 1 : 0), Math.Round(Max, 0, MidpointRounding.ToPositiveInfinity).ToString().Length - (Max < 0 ? 1 : 0)))}";

            if (_precision > 0)
            {
                _formatString += "." + new string('0', _precision);
            }

            _formatString += '}';
        }

        /// <inheritdoc/>
        public override void SetValue(object? value)
        {
            if (value == null || value as string == "null")
            {
                _state = null;
                _stateJustification = null;
                _displayValue = null;
                _decorated.SetValue(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                _state = null;
                _stateJustification = null;

                var cleanValue = Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision);
                _displayValue = string.Format(_formatString, cleanValue);

                _decorated.SetValue(cleanValue);
            }
            else
            {
                _state = ModuleState.Error;
                _stateJustification = $"Value cannot be cast as a double";
                _displayValue = null;
                _decorated.SetValue(value);
            }
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            if (value == null || value as string == "null")
            {
                _state = null;
                _stateJustification = null;
                _displayValue = null;

                return _decorated.TrySetValue(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                _state = null;
                _stateJustification = null;

                var cleanValue = Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision);
                _displayValue = string.Format(_formatString, cleanValue);

                if (_decorated.TrySetValue(cleanValue))
                {
                    _displayValue = string.Format(_formatString, cleanValue);
                    return true;
                }
                else
                {
                    _displayValue = null;
                    return false;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override void SetValueFromRead(object? value)
        {
            if (value == null || value as string == "null")
            {
                _state = null;
                _stateJustification = null;
                _displayValue = null;
                _decorated.SetValueFromRead(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                _state = null;
                _stateJustification = null;

                var cleanValue = Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision);
                _displayValue = string.Format(_formatString, cleanValue);

                _decorated.SetValueFromRead(cleanValue);
            }
            else
            {
                _state = ModuleState.Error;
                _stateJustification = $"Value cannot be cast as a double";
                _displayValue = null;
                _decorated.SetValueFromRead(value);
            }
        }

        /// <inheritdoc/>
        public override bool TrySetValueFromRead(object? value)
        {
            if (value == null || value as string == "null")
            {
                _state = null;
                _stateJustification = null;
                _displayValue = null;

                return _decorated.TrySetValueFromRead(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                _state = null;
                _stateJustification = null;

                var cleanValue = Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision);

                if (_decorated.TrySetValueFromRead(cleanValue))
                {
                    _displayValue = string.Format(_formatString, cleanValue);
                    OnPropertyChanged(nameof(DisplayValue));
                    return true;
                }
                else
                {
                    _displayValue = null;
                    OnPropertyChanged(nameof(DisplayValue));
                    return false;
                }
            }

            return false;
        }
    }
}
