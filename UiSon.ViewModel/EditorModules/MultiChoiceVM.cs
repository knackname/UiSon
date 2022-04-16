using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;

namespace UiSon.ViewModel
{
    public class MultiChoiceVM : GroupVM
    {
        private Type _collectionType;
        private Type _memberType;
        private MethodInfo _collectionAdd;
        private ValueMemberInfo _info;
        private List<CheckboxVM> _members;

        public MultiChoiceVM(List<CheckboxVM> members, Type collectionType, Type memberType, ValueMemberInfo info = null, string name = null, int priority = 0,
                             DisplayMode displayMode = DisplayMode.Horizontal)
            :base(members,name,priority,displayMode, true)
        {
            _members = members ?? throw new ArgumentNullException(nameof(members));
            _collectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            _memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
            _info = info;

            _collectionAdd = _collectionType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                .GetMethod("Add");
        }

        public override void Read(object instance) => SetValue(_info.GetValue(instance));

        public override void Write(object instance)
        {
            var value = Activator.CreateInstance(_collectionType);

            foreach (var member in _members.Where(x => x.Value))
            {
                if ((bool)member.GetValueAs(typeof(bool)))
                {
                    _collectionAdd.Invoke(value, new[] { member.Name.ParseAs(_memberType) });
                }
            }

            _info.SetValue(instance, value);
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
                    _members.FirstOrDefault(x => x.Name == n.ToString())?.SetValue(true);
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
