// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Reflection;
using UiSon.Element.Element.Interface;

namespace UiSon.Element
{
    public class RefElement : GroupElement
    {
        private ConstructorInfo _constructor;
        private MemberInfo _info;

        public RefElement(string name, int priority, IEnumerable<IElement> members,
            MemberInfo info, ConstructorInfo constructor)
            :base(name, priority, members)
        {
            _info = info ?? throw new ArgumentNullException(nameof(info));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        public override void Write(object instance)
        {
            var value = _constructor.Invoke(null);

            base.Write(value);

            if (_info is PropertyInfo prop)
            {
                prop.SetValue(instance, value);
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
            else
            {
                throw new Exception("Attempting to write on an element without member info");
            }
        }

        public override void Read(object instance)
        {
            object value = null;

            if (_info is PropertyInfo prop)
            {
                value = prop.GetValue(instance);
            }
            else if (_info is FieldInfo field)
            {
                value = field.GetValue(instance);
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }

            base.Read(value);
        }
    }
}
