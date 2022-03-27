using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UiSon.Attribute;

namespace UiSon.ViewModel
{
    public class MultiChoiceVM : GroupVM
    {
        private Type _collectionType;
        private Type _memberType;
        private MethodInfo _collectionAdd;
        private MemberInfo _info;
        private List<CheckboxVM> _members;

        public MultiChoiceVM(List<CheckboxVM> members, Type collectionType, Type memberType, MemberInfo info = null, string name = null, int priority = 0,
                             DisplayMode displayMode = DisplayMode.Vertial)
            :base(members,name,priority,displayMode)
        {
            _members = members ?? throw new ArgumentNullException(nameof(members));
            _collectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            _info = info;

            _collectionAdd = _collectionType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                .GetMethod("Add");
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

            var value = Activator.CreateInstance(_collectionType);

            foreach (var member in _members.Where(x => x.Value))
            {
                _collectionAdd.Invoke(value, new[] { member.GetValueAs(_memberType)});
            }

            if (_info is PropertyInfo prop)
            {
                prop.GetSetMethod(true).Invoke(instance, new[] { value });
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

        public override bool SetValue(object value)
        {
            if (value != null && value.GetType().GetInterface(nameof(IEnumerable)) != null)
            {
                // set them all to false
                foreach (var member in _members)
                {
                    member.SetValue(false);
                }

                // set the ones in the list to true
                foreach (var n in value as IEnumerable)
                {
                    var found = _members.FirstOrDefault(x => x.Name == n.ToString());
                }

                OnPropertyChanged(nameof(Members));

                return true;
            }

            return false;
        }

        public override object GetValueAs(Type type)
        {
            if (type != _collectionType)
            {
                return null;
            }

            var instance = Activator.CreateInstance(_collectionType);

            foreach (var member in _members.Where(x => x.Value))
            {
                _collectionAdd.Invoke(instance, new[] { member.GetValueAs(_memberType) });
            }

            return instance;
        }
    }
}
