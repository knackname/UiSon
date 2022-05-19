// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;

namespace UiSon.Element
{
    /// <summary>
    /// Decorates <see cref="MemberInfo"/> either a field or property. Provides common interface for get and set value.
    /// </summary>
    public class ValueMemberInfo : MemberInfo
    {
        private readonly string NotFieldOrPropertyError = $"Wrapped {nameof(MemberInfo)} wasn't a {nameof(FieldInfo)} or a {nameof(PropertyInfo)}.";

        /// <inheritdoc cref="MemberInfo.DeclaringType" />
        public override Type DeclaringType => _decorated.DeclaringType;

        /// <inheritdoc cref="MemberInfo.MemberType" />
        public override MemberTypes MemberType => _decorated.MemberType;

        /// <inheritdoc cref="MemberInfo.Name" />
        public override string Name => _decorated.Name;

        /// <inheritdoc cref="MemberInfo.ReflectedType" />
        public override Type ReflectedType => _decorated.ReflectedType;

        /// <summary>
        /// The decorated <see cref="MemberInfo"/>.
        /// </summary>
        private readonly MemberInfo _decorated;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="decorated">The decorated <see cref="MemberInfo"/>.</param>
        public ValueMemberInfo(MemberInfo decorated)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));

            switch (_decorated.MemberType)
            {
                case MemberTypes.Field:
                case MemberTypes.Property:
                    break;
                default:
                    throw new Exception(NotFieldOrPropertyError);
            }
        }

        /// <summary>
        /// Gets the value of the instance's value member.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>The value of the instance's value member.</returns>
        public object GetValue(object instance)
        {
            switch (_decorated.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)_decorated).GetValue(instance);
                case MemberTypes.Property:
                    return ((PropertyInfo)_decorated).GetValue(instance);
                default:
                    throw new Exception(NotFieldOrPropertyError);
            }
        }

        /// <summary>
        /// Sets the instances value member to the value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object instance, object value)
        {
            switch (_decorated.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)_decorated).SetValue(instance, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)_decorated).GetSetMethod(true)?.Invoke(instance, new[] { value });
                    break;
                default:
                    throw new Exception(NotFieldOrPropertyError);
            }
        }

        /// <inheritdoc cref="MemberInfo.GetCustomAttributes(bool)" />
        public override object[] GetCustomAttributes(bool inherit) => _decorated.GetCustomAttributes(inherit);

        /// <inheritdoc cref="MemberInfo.GetCustomAttributes(Type, bool)" />
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _decorated.GetCustomAttributes(attributeType, inherit);

        /// <inheritdoc cref="MemberInfo.IsDefined(Type, bool)" />
        public override bool IsDefined(Type attributeType, bool inherit) => _decorated.IsDefined(attributeType, inherit);
    }
}
