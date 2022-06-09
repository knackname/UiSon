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
        /// <returns>True if successful, false otherwise.</returns>
        public static bool TryCast(this object value, Type type, out object result)
        {
            if (type == null)
            {
                result = null;
                return false;
            }
            else if (value == null)
            {
                result = null;
                return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
            }
            else if (type.IsEnum
                     && value.TryCast(typeof(int), out object asInt)
                     && Enum.IsDefined(type, asInt))
            {
                result = Enum.ToObject(type, asInt);
                return true;
            }
            else if (value is string parseString)
            {
                result = parseString.ParseAs(type);
                return result != null;
            }
            else if (type == typeof(string))
            {
                result = value.ToString();
                return true;
            }
            else if (type.IsValueType && value.GetType().IsValueType)
            {
                type = Nullable.GetUnderlyingType(type) ?? type;
                result = Convert.ChangeType(value, type);
                return true;
            }

            result = null;
            return false;
        }
    }
}
