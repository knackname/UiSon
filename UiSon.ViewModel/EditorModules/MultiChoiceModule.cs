// UiSon, by Cameron Gale 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class MultiChoiceModule : GroupModule
    {
        public override object Value
        {
            get
            {
                var value = Activator.CreateInstance(_collectionType);

                // some implimentations of ICollection<> may have rules about what can be added and throw an error
                // in that case refresh the old value back into the child moduels
                try
                {
                    foreach (var member in Members)
                    {
                        if ((bool)member.Value)
                        {
                            _collectionAdd.Invoke(value, new[] { member.Name.ParseAs(_entryType) });
                        }
                    }

                    _isValueBad = false;
                    return value;
                }
                catch
                {
                    _isValueBad = true;
                    return null;
                }
            }
            set
            {
                if (value is IEnumerable enumerable
                    && value.GetType()
                            .GetInterfaces()
                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                            .GetGenericArguments()
                            .FirstOrDefault() == _entryType)
                {
                    foreach (var member in Members)
                    {
                        member.Value = false;
                    }

                    foreach (var entry in enumerable)
                    {
                        var member = Members.FirstOrDefault(x => x.Name == entry.ToString());

                        if (member != null)
                        {
                            member.Value = true;
                        }
                    }

                    _isValueBad = true;
                    OnPropertyChanged(nameof(Members));
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        private bool _isValueBad;

        public override ModuleState State => _isValueBad ? ModuleState.Error : base.State;

        private readonly Type _collectionType;
        private readonly Type _entryType;
        private readonly MethodInfo _collectionAdd;
        private readonly ValueMemberInfo _info;

        public MultiChoiceModule(ValueMemberInfo info,
                                 NotifyingCollection<CheckboxModule> members,
                                 Type collectionType,
                                 Type entryType,
                                 string name,
                                 int priority,
                                 DisplayMode displayMode)
            :base(members, name, priority, displayMode)
        {
            _info = info;
            _collectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));

            members.PropertyChanged += OnMembersPropertyChanged;

            _collectionAdd = _collectionType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                .GetMethod("Add");
        }

        private void OnMembersPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IEditorModule.Value):
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        public override void Read(object instance) => Value = _info.GetValue(instance);

        public override void Write(object instance) => _info.SetValue(instance, Value);
    }
}
