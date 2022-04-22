using System;
using System.Reflection;

namespace UiSon.Element
{
    /// <summary>
    /// member info of either a field or property. Provides common interface for value
    /// </summary>
    public class ValueMemberInfo : MemberInfo
    {
        public override Type DeclaringType => _decorated.DeclaringType;
        public override MemberTypes MemberType => _decorated.MemberType;
        public override string Name => _decorated.Name;
        public override Type ReflectedType => _decorated.ReflectedType;

        private MemberInfo _decorated;

        // one will be set, the other will be null
        private FieldInfo _asField;
        private PropertyInfo _asProperty;

        public ValueMemberInfo(FieldInfo decorated)
        {
            _decorated = _asField = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }

        public ValueMemberInfo(PropertyInfo decorated)
        {
            _decorated = _asProperty = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }

        public ValueMemberInfo(MemberInfo decorated)
        {
            if (decorated is FieldInfo field)
            {
                _decorated = _asField = field;
            }
            else if (decorated is PropertyInfo property)
            {
                _decorated = _asProperty = property;
            }
            else
            {
                throw new ArgumentException(nameof(decorated));
            }
        }

        public object GetValue(object source) => _asField == null
            ? _asProperty.GetValue(source)
            : _asField.GetValue(source);

        public void SetValue(object instance, object value)
        {
            if (_asField == null)
            {
                _asProperty.GetSetMethod(true).Invoke(instance, new[] { value });
            }
            else
            {
                _asField.SetValue(instance, value);
            }
        }

        public override object[] GetCustomAttributes(bool inherit) => _decorated.GetCustomAttributes(inherit);
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => _decorated.GetCustomAttributes(attributeType, inherit);
        public override bool IsDefined(Type attributeType, bool inherit) => _decorated.IsDefined(attributeType, inherit);
    }
}
