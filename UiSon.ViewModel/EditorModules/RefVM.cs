// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Reflection;
using UiSon.Attribute;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class RefVM : GroupVM
    {
        private MemberInfo _info;

        public RefVM(IEnumerable<IEditorModule> members,
            MemberInfo info,
            string name = null,
            int priority = 0,
            DisplayMode displayMode = DisplayMode.Vertial)
            : base(members, name, priority, displayMode)
        {
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

        public override bool SetValue(object value)
        {
            base.Read(value);

            return true;
        }
    }
}
