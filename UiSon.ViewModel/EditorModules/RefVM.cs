// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class RefVM : GroupVM
    {
        private Type _type;
        private ValueMemberInfo _info;

        public RefVM(IEnumerable<IEditorModule> members,
            Type type,
            ValueMemberInfo info,
            string name,
            int priority,
            DisplayMode displayMode)
            : base(members, name, priority, displayMode)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _info = info;
        }

        public override void Read(object instance) => SetValue(_info.GetValue(instance));

        public override void Write(object instance)
        {
            var subInstance = Activator.CreateInstance(_type);

            base.Write(subInstance);

            _info.SetValue(instance, subInstance);
        }

        public override bool SetValue(object value)
        {
            base.Read(value);
            return true;
        }

        public override object GetValueAs(Type type)
        {
            if (type.IsAssignableFrom(_type))
            {
                var instance = Activator.CreateInstance(_type);
                base.Write(instance);
                return instance;
            }

            return null;
        }
    }
}
