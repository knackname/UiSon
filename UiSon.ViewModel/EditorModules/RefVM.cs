// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Reflection;
using UiSon.Attribute;
using UiSon.Extension;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class RefVM : GroupVM
    {
        private Type _type;
        private MemberInfo _info;

        public RefVM(IEnumerable<IEditorModule> members,
            Type type,
            MemberInfo info,
            string name = null,
            int priority = 0,
            DisplayMode displayMode = DisplayMode.Vertial)
            : base(members, name, priority, displayMode)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _info = info;
        }

        public override void Read(object instance)
        {
            if (instance == null) { return; }

            if (_info is PropertyInfo prop)
            {
                SetValue(prop.GetValue(instance));
            }
            else if (_info is FieldInfo field)
            {
                SetValue(field.GetValue(instance));
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }
        }

        public override void Write(object instance)
        {
            if (instance == null) { return; }

            var subInstance = Activator.CreateInstance(_type);

            base.Write(subInstance);

            if (_info is PropertyInfo prop)
            {
                prop.SetValue(instance, subInstance);
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, subInstance);
            }
            else
            {
                throw new Exception("Attempting to write on an element without member info");
            }
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
