// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;

namespace UiSon.Extension
{
    public static partial class ExtendString
    {        
        /// <summary>
        /// Attempts to parse the string as the type. Returns null if unsuccessful
        /// </summary>
        /// <param name="value">This string</param>
        /// <param name="type">Type to parse as</param>
        /// <returns>the parsed string or null if unsuccessful</returns>
        public static object ParseAs(this string value, Type type)
        {
            if (type == null)  { return null; }

            // strip nullable value types
            type = Nullable.GetUnderlyingType(type) ?? type;

            // seperate because they're not a specific value type
            if (type.IsEnum)
            {
                return Enum.TryParse(type, value, out var asEnum) ? asEnum : null;
            }

            // value types
            if (TypeLookup.TypeToId.ContainsKey(type))
            {
                switch (TypeLookup.TypeToId[type])
                {
                    case ValueishType._string:
                        return value;
                    case ValueishType._sbyte:
                        return sbyte.TryParse(value, out var asSbyte) ? asSbyte : null;
                    case ValueishType._byte:
                        return byte.TryParse(value, out var asByte) ? asByte : null;
                    case ValueishType._short:
                        return short.TryParse(value, out var asShort) ? asShort : null;
                    case ValueishType._ushort:
                        return ushort.TryParse(value, out var asUshort) ? asUshort : null;
                    case ValueishType._int:
                        return int.TryParse(value, out var asInt) ? asInt : null;
                    case ValueishType._uint:
                        return uint.TryParse(value, out var asUint) ? asUint : null;
                    case ValueishType._long:
                        return long.TryParse(value, out var asLong) ? asLong : null;
                    case ValueishType._ulong:
                        return ulong.TryParse(value, out var asUlong) ? asUlong : null;
                    case ValueishType._nint:
                        return nint.TryParse(value, out var asNint) ? asNint : null;
                    case ValueishType._nuint:
                        return nuint.TryParse(value, out var asNunit) ? asNunit : null;
                    case ValueishType._float:
                        return float.TryParse(value, out var asFloat) ? asFloat : null;
                    case ValueishType._double:
                        return double.TryParse(value, out var asDOuble) ? asDOuble : null;
                    case ValueishType._decimal:
                        return decimal.TryParse(value, out var asDecimal) ? asDecimal : null;
                    case ValueishType._bool:
                        return bool.TryParse(value, out var asBool) ? asBool : null;
                    case ValueishType._char:
                        return value[0];
                    case ValueishType._type:
                        return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
