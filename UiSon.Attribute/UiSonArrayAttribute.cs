// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Linq;

namespace UiSon.Attribute
{
    /// <summary>
    /// Defines a string array to be used by other UiSon attributes.
    /// Used to define a common string array for multiple parameters.
    /// The array will be available to all UiSon attributes in the assembly,
    /// not just those in the class on which it's defined.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UiSonArrayAttribute : System.Attribute
    {
        /// <summary>
        /// The array's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The array.
        /// </summary>
        public string[] Array { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The array's name.</param>
        /// <param name="array">The array.</param>
        public UiSonArrayAttribute(string name, object[] array)
        {
            Name = name;
            Array = array?.Select(x => x?.ToString()).ToArray();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The array's name.</param>
        /// <param name="enumType">The enum type to extrace the array from.</param>
        /// <param name="castAs">The type to cast the enum to. Must be a value type or the string type.</param>
        public UiSonArrayAttribute(string name, Type enumType, Type castAs)
        {
            Name = name;

            if (enumType != null)
            {
                var list = new List<string>();

                var uType = Nullable.GetUnderlyingType(enumType);
                if (uType != null)
                {
                    enumType = uType;
                    list.Add("null");
                }

                if (enumType.IsEnum)
                {
                    if (castAs == null || castAs == typeof(string))
                    {
                        foreach (var entry in Enum.GetNames(enumType))
                        {
                            list.Add(entry);
                        }
                    }
                    else if (castAs.IsValueType)
                    {
                        foreach (var a in Enum.GetValues(enumType))
                        {
                            list.Add(Convert.ChangeType(a, castAs).ToString());
                        }

                        Array = list.ToArray();
                    }
                }

                Array = list.ToArray();
            }
        }
    }
}
