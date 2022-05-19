using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UiSon.Extension
{
    public static partial class ExtendType
    {
        /// <summary>
        /// Returns an enumerable of the type's public and private field and property members
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetAllDataAccessInstances(this Type value)
            => value.GetAllFieldInstances()
                    .OfType<MemberInfo>()
                    .Concat(value.GetAllPropertyInstances()
                                 .OfType<MemberInfo>());

        /// <summary>
        /// Returns an enumerable of the type's public and private fields
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetAllFieldInstances(this Type value)
            => value.GetFields(BindingFlags.Public
                               | BindingFlags.NonPublic
                               | BindingFlags.Instance);

        /// <summary>
        /// Returns an enumerable of the type's public and private properties
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetAllPropertyInstances(this Type value)
            => value.GetProperties(BindingFlags.Public
                                   | BindingFlags.NonPublic
                                   |BindingFlags.Instance);
    }
}
