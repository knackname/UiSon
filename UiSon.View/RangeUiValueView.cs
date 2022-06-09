// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class RangeUiValueView : NPCBase, IRangeView
    {
        /// <inheritdoc/>
        public object? Value => StringValue.ParseAs(_decorated.Type);

        /// <inheritdoc/>
        public object? DisplayValue => StringValue;

        /// <summary>
        /// The value as a string
        /// </summary>
        private string? StringValue => _decorated.Value.TryCast(typeof(double), out object casted) ? string.Format(_formatString, casted) : null;

        /// <summary>
        /// Minimum value
        /// </summary>
        public double Min { get; private set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public double Max { get; private set; }

        /// <inheritdoc/>
        public bool IsValueBad
        {
            get
            {
                if (Value == null || Value as string == "null")
                {
                    return _decorated.IsValueBad;
                }
                else if (Value.TryCast(typeof(double), out object asDouble))
                {
                    return (double)asDouble > Max || (double)asDouble < Min;
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public string? Name => _decorated.Name;

        /// <inheritdoc/>
        public int Percision => _precision;

        /// <inheritdoc/>
        public UiType UiType => _decorated.UiType;

        /// <inheritdoc/>
        public Type? Type => _decorated.Type;

        /// <summary>
        /// If the ui should be vertical rather than horizontal
        /// </summary>
        public bool IsVertical { get; private set; }

        private readonly int _precision;
        private readonly string _formatString;
        private readonly IUiValueView _decorated;
        private readonly ValueMemberInfo? _info;

        public RangeUiValueView(IUiValueView decorated, double min, double max, int percision, bool isVertical, ValueMemberInfo? info)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;

            Min = min;
            Max = max;
            _precision = percision;
            IsVertical = isVertical;
            _info = info;

            _formatString = '{' + $"0:{new string('0', Math.Max(Math.Round(Min, 0, MidpointRounding.ToNegativeInfinity).ToString().Length - (Min < 0 ? 1 : 0), Math.Round(Max, 0, MidpointRounding.ToPositiveInfinity).ToString().Length - (Max < 0 ? 1 : 0)))}";

            if (_precision > 0)
            {
                _formatString += "." + new string('0', _precision);
            }

            _formatString += '}';
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
                case nameof(IUiValueView.UiType):
                    OnPropertyChanged(nameof(UiType));
                    break;
            }
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            if (value == null || value as string == "null")
            {
                return _decorated.TrySetValue(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                return _decorated.TrySetValue(Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision));
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            if (value == null || value as string == "null")
            {
                return _decorated.TrySetValueFromRead(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                return _decorated.TrySetValueFromRead(Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision));
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

            _info.SetValue(instance, Value);
        }
    }
}
