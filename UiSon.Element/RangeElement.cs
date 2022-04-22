// UiSon, by Cameron Gale 2022

using System;
using UiSon.Extension;

namespace UiSon.Element
{
    public class RangeElement : ValueElement<double>, IUiSonElement
    {
        public double Min { get; private set; }

        public double Max { get; private set; }

        private int _precision;
        private string _formatString;

        public RangeElement(Type memberType, ValueMemberInfo info, double min, double max, int percision)
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

        public override object GetValueAs(Type type)
        {
            if (type == typeof(string))
            {
                var asDouble = base.GetValueAs(typeof(double?));
                return asDouble == null ? null : string.Format(_formatString, asDouble);
            }

            return base.GetValueAs(type);
        }

        public override bool SetValue(object value)
        {
            if (value is string asString && asString == "null")
            {
                return base.SetValue(null);
            }
            else if (value.TryCast(typeof(double), out var asDouble))
            {
                return base.SetValue(Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision));
            }

            return false;
        }
    }
}
