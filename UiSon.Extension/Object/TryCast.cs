// UiSon, by Cameron Gale 2022

using System;

namespace UiSon.Extension
{
    public static partial class ExtendObject
    {
        /// <summary>
        /// Attempts to cast the value to the type
        /// </summary>
        /// <param name="value">The value to cast</param>
        /// <param name="type">The type to cast to</param>
        /// <param name="result">The cast value</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool TryCast(this object value, Type type, out object result)
        {
            if (type == null)
            {
                result = null;
                return false;
            }
            else if (type.IsEnum &&
                value is int enumInt
                && Enum.IsDefined(type, enumInt))
            {
                // no need to actually cast to the enum... it'll just have to be cast again on the other side.
                result = enumInt;
                return true;
            }
            else if (value is string parseString)
            {
                result = parseString.ParseAs(type);
                return result != null;
            }
            else if (TypeLookup.TypeToId.ContainsKey(type))
            {
                switch (TypeLookup.TypeToId[type])
                {
                    case ValueishType._string:
                        result = value.ToString();
                        return true;
                    case ValueishType._sbyte:
                        if (value is sbyte asSbyte)
                        {
                            result = asSbyte;
                            return true;
                        }
                        break;
                    case ValueishType._byte:
                        if (value is byte asByte)
                        {
                            result = asByte;
                            return true;
                        }
                        break;
                    case ValueishType._short:
                        if (value is short asShort)
                        {
                            result = asShort;
                            return true;
                        }
                        break;
                    case ValueishType._ushort:
                        if (value is ushort asUshort)
                        {
                            result = asUshort;
                            return true;
                        }
                        break;
                    case ValueishType._int:
                        if (value is int asInt)
                        {
                            result = asInt;
                            return true;
                        }
                        break;
                    case ValueishType._uint:
                        if (value is uint asUint)
                        {
                            result = asUint;
                            return true;
                        }
                        break;
                    case ValueishType._long:
                        if (value is long asLong)
                        {
                            result = asLong;
                            return true;
                        }
                        break;
                    case ValueishType._ulong:
                        if (value is ulong asUlong)
                        {
                            result = asUlong;
                            return true;
                        }
                        break;
                    case ValueishType._nint:
                        if (value is nint asNint)
                        {
                            result = asNint;
                            return true;
                        }
                        break;
                    case ValueishType._nuint:
                        if (value is nuint asNuint)
                        {
                            result = asNuint;
                            return true;
                        }
                        break;
                    case ValueishType._float:
                        if (value is float asFloat)
                        {
                            result = asFloat;
                            return true;
                        }
                        break;
                    case ValueishType._double:
                        if (value is double asDouble)
                        {
                            result = asDouble;
                            return true;
                        }
                        break;
                    case ValueishType._decimal:
                        if (value is decimal asDecimal)
                        {
                            result = asDecimal;
                            return true;
                        }
                        break;
                    case ValueishType._bool:
                        if (value is bool asBool)
                        {
                            result = asBool;
                            return true;
                        }
                        break;
                    case ValueishType._char:
                        if (value is char asChar)
                        {
                            result = asChar;
                            return true;
                        }
                        break;
                }
            }

            result = value;
            return false;
        }
    }
}
