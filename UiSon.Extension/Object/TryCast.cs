// UiSon, by Cameron Gale 2022

using System;
using System.Reflection;

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
            else if (type == value.GetType())
            {
                result = value;
                return true;
            }

            // then check for explicit conversions
            foreach (var mi in type.GetMethods())
            {
                if (mi.Name == "op_Explicit"
                    && mi is MethodInfo method)
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length == 1
                        && parameters[0].ParameterType == value.GetType())
                    {
                        result = method.Invoke(null, new[] { value });
                        return true;
                    }
                }
            }

            // then implicite conversions
            foreach (var mi in value.GetType().GetMethods())
            {
                if (mi.Name == "op_Implicit"
                    && mi is MethodInfo method
                    && method.ReturnType == type)
                {
                    result = method.Invoke(null, new[] { value });
                    return true;
                }
            }

            // then try primitive-ish types
            if (type.IsEnum && Enum.IsDefined(type, value))
            {
                if (value.TryCast(typeof(int), out object asInt))
                {
                    result = Enum.ToObject(type, asInt);
                }
                else if (value is string asString)
                {
                    result = asString.ParseAs(type);
                }
                else
                {
                    result = null;
                    return false;
                }

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
