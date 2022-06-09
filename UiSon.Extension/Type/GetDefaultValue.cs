using System;

namespace UiSon.Extension
{
    public static partial class ExtendType
    {
        /// <summary>
        /// Returns a default instance of the type.
        /// </summary>
        /// <returns></returns>
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
            else if (value.IsValueType || value.GetConstructor(new Type[] { }) != null)
            {
                return Activator.CreateInstance(value);
            }

            return null;
        }
    }
}
