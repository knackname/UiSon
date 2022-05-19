using System;

namespace UiSon.Extension
{
    public static partial class ExtendType
    {
        /// <summary>
        /// Returns a default instance of the type.
        /// </summary>
        /// <returns></returns>
        public static object GetDefaultValue(this Type value) => (value.IsValueType || value.GetConstructor(new Type[] { }) != null)
                                                                  ? Activator.CreateInstance(value)
                                                                  : null;
    }
}
