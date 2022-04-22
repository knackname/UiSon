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

            _formatString = '{' + $"0:{new string('0', Math.Max(Math.Round(Min, 0, MidpointRounding.ToNegativeInfinity).ToString().Length, Math.Round(Max, 0, MidpointRounding.ToPositiveInfinity).ToString().Length))}";

            if (_precision > 0)
            {
                _formatString += "." + new string('0', _precision);
            }

            _formatString += '}';

            SetValue((Min+Max)/2d);
        }

        public override object GetValueAs(Type type)
        {
            if (type == typeof(string))
            {
                return string.Format(_formatString, base.GetValueAs(typeof(double)));
            }

            return base.GetValueAs(type);
        }

        public override bool SetValue(object value)
        {
            if (value.TryCast(typeof(double), out var asDouble))
            {
                return base.SetValue(Math.Round(Math.Max(Min, Math.Min(Max, (double)asDouble)), _precision));
            }
            return false;
        }
    }
}
