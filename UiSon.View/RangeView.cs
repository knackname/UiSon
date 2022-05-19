// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class RangeView : ValueView<double>, IReadWriteView
    {
        /// <inheritdoc/>
        public override object? Value => base.Value == null ? string.Format(_formatString, (double)base.Value) : null;

        /// <summary>
        /// Minimum value
        /// </summary>
        public double Min { get; private set; }

        /// <summary>
        /// Maximum value
        /// </summary>
        public double Max { get; private set; }

        private int _precision;
        private string _formatString;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="memberType"></param>
        /// <param name="info"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="percision"></param>
        public RangeView(Type memberType, ValueMemberInfo info, double min, double max, int percision)
            :base(memberType, info)
        {
            Min = min;
            Max = max;
            _precision = percision;

            _formatString = '{' + $"0:{new string('0', Math.Max(Math.Round(Min, 0, MidpointRounding.ToNegativeInfinity).ToString().Length - (Min < 0 ? 1 : 0), Math.Round(Max, 0, MidpointRounding.ToPositiveInfinity).ToString().Length - (Max < 0 ? 1 : 0)))}";

            if (_precision > 0)
            {
                _formatString += "." + new string('0', _precision);
            }

            _formatString += '}';
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            if (value is string asString && asString == "null")
            {
                return base.TrySetValue(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                return base.TrySetValue(Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision));
            }

            return false;
        }
    }
}
