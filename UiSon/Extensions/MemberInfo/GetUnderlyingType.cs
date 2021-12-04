// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;

namespace UiSon.Extensions
{
    /// <summary>
    /// Gets the type of the member
    /// </summary>
    public static partial class ExtendMemberInfo
    {
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    return null;
            }
        }
    }
}
