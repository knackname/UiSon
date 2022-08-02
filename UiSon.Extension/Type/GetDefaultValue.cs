using System;

namespace UiSon.Extension
{
    public static partial class ExtendType
    {
        /// <summary>
        /// Returns a default instance of the type.
        /// Empty string for string,
        /// 0 legnth array for arrays,
        /// Default for value types,
        /// Paramaterless constructor for class types that have one,
        /// Null otherwise
        /// </summary>
        public static object GetDefaultValue(this Type value)
        {
            if (value == typeof(string))
            {
                return string.Empty;
            }
            else if (value.IsArray)
            {
                return Array.CreateInstance(value.GetElementType(), 0);
            }
            else if (value.IsValueType)
            {
                return Activator.CreateInstance(value);
            }

            return value.GetConstructor(Array.Empty<Type>())?.Invoke(null);
        }
    }
}
