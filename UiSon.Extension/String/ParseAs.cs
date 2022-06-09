// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UiSon.Extension
{
    public static partial class ExtendString
    {
        /// <summary>
        /// Identifies types for a switch statement
        /// </summary>
        public static IReadOnlyDictionary<Type, PrimativishType> TypeToId = new Dictionary<Type, PrimativishType>
        {
            { typeof(string), PrimativishType._string},
            { typeof(sbyte), PrimativishType._sbyte},
            { typeof(byte), PrimativishType._byte},
            { typeof(short), PrimativishType._short},
            { typeof(ushort), PrimativishType._ushort},
            { typeof(int), PrimativishType._int},
            { typeof(uint), PrimativishType._uint},
            { typeof(long), PrimativishType._long},
            { typeof(ulong), PrimativishType._ulong},
            { typeof(nint), PrimativishType._nint},
            { typeof(nuint), PrimativishType._nuint},
            { typeof(float), PrimativishType._float},
            { typeof(double), PrimativishType._double},
            { typeof(decimal), PrimativishType._decimal},
            { typeof(bool), PrimativishType._bool},
            { typeof(char), PrimativishType._char},
            { typeof(Type), PrimativishType._type},
        };

        /// <summary>
        /// Attempts to parse the string as the type. Returns null if unsuccessful
        /// </summary>
        /// <param name="value">This string</param>
        /// <param name="type">Type to parse as</param>
        /// <returns>the parsed string or null if unsuccessful</returns>
        public static object ParseAs(this string value, Type type)
        {
            if (type == null || value == null)  { return null; }

            // strip nullable value types
            type = Nullable.GetUnderlyingType(type) ?? type;

            // seperate because they're not a specific value type
            if (type.IsEnum)
            {
                return Enum.TryParse(type, value, out var asEnum) ? asEnum : null;
            }

            // Primativish types
            if (TypeToId.ContainsKey(type))
            {
                switch (TypeToId[type])
                {
                    case PrimativishType._string:
                        return value;
                    case PrimativishType._sbyte:
                        return sbyte.TryParse(value, out var asSbyte) ? asSbyte : null;
                    case PrimativishType._byte:
                        return byte.TryParse(value, out var asByte) ? asByte : null;
                    case PrimativishType._short:
                        return short.TryParse(value, out var asShort) ? asShort : null;
                    case PrimativishType._ushort:
                        return ushort.TryParse(value, out var asUshort) ? asUshort : null;
                    case PrimativishType._int:
                        return int.TryParse(value, out var asInt) ? asInt : null;
                    case PrimativishType._uint:
                        return uint.TryParse(value, out var asUint) ? asUint : null;
                    case PrimativishType._long:
                        return long.TryParse(value, out var asLong) ? asLong : null;
                    case PrimativishType._ulong:
                        return ulong.TryParse(value, out var asUlong) ? asUlong : null;
                    case PrimativishType._nint:
                        return nint.TryParse(value, out var asNint) ? asNint : null;
                    case PrimativishType._nuint:
                        return nuint.TryParse(value, out var asNunit) ? asNunit : null;
                    case PrimativishType._float:
                        return float.TryParse(value, out var asFloat) ? asFloat : null;
                    case PrimativishType._double:
                        return double.TryParse(value, out var asDOuble) ? asDOuble : null;
                    case PrimativishType._decimal:
                        return decimal.TryParse(value, out var asDecimal) ? asDecimal : null;
                    case PrimativishType._bool:
                        return bool.TryParse(value, out var asBool) ? asBool : null;
                    case PrimativishType._char:
                        return value.Length > 0 ? value[0] : null;
                    case PrimativishType._type:
                        return TypeDescriptor.GetConverter(type).ConvertFromInvariantString(value);
                    default:
                        return null;
                }
            }

            return null;
        }
    }
}
