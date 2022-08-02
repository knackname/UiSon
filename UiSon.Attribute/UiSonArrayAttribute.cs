// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;

namespace UiSon.Attribute
{
    /// <summary>
    /// Defines an array to be used by other UiSon attributes.
    /// Used to define a common array for multiple parameters.
    /// The array will be available to all UiSon attributes in the assembly,
    /// not just those in the class on which it's defined.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = true)]
    public class UiSonArrayAttribute : System.Attribute
    {
        /// <summary>
        /// The array's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The array.
        /// </summary>
        public object[] Array { get; private set; }

        /// <summary>
        /// The array.
        /// </summary>
        public Type JsonDeserializeType { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The array's name.</param>
        /// <param name="array">The array.</param>
        public UiSonArrayAttribute(string name, object[] array, Type jsonDeserializeType = null)
        {
            Name = name;
            Array = array;
            JsonDeserializeType = jsonDeserializeType;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The array's name.</param>
        /// <param name="enumType">The enum type to extrace the array from.</param>
        public UiSonArrayAttribute(string name, Type enumType)
        {
            Name = name;

            if (enumType != null)
            {
                var list = new List<object>();

                var uType = Nullable.GetUnderlyingType(enumType);
                if (uType != null)
                {
                    enumType = uType;
                    list.Add("null");
                }

                if (enumType.IsEnum)
                {
                    foreach (var enumValue in Enum.GetValues(enumType))
                    {
                        list.Add(enumValue);
                    }
                }

                Array = list.ToArray();
            }
            else
            {
                Array = System.Array.Empty<string>();
            }
        }
    }
}
