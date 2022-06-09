// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;
using UiSon.Element;

namespace UiSon.Extension
{
    public static partial class ExtendMemberInfo
    {
        /// <summary>
        /// Gets the type of the member
        /// </summary>
        public static Type GetUnderlyingType(this MemberInfo memberInfo)
        {
            if (memberInfo is ValueMemberInfo valueMemberInfo)
            {
                return valueMemberInfo.ValueType;
            }

            switch (memberInfo.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)memberInfo).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)memberInfo).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).PropertyType;
                default:
                    return null;
            }
        }
    }
}
