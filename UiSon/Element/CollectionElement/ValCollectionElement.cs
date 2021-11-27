// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Attribute.Enums;
using UiSon.Commands;
using UiSon.Extensions;

namespace UiSon.Element
{
    public class ValCollectionElement<T> : CollectionElement<ValCollectionEntry<T>>
    {
        private ElementFactory _factory;

        private UiSonAttribute _entryAttribute;

        private MemberInfo _info;

        public ValCollectionElement(string name, int priority,
                                    DisplayMode displayMode, bool modifiable, CollectionType collectionType, Alignment alignment,
                                    UiSonAttribute entryAttribute, MemberInfo info, ElementFactory factory)
            : base(name, priority, displayMode, modifiable, collectionType, alignment)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _entryAttribute = entryAttribute ?? throw new ArgumentNullException(nameof(entryAttribute));

            _info = info ?? throw new ArgumentNullException(nameof(info));
        }

        public override ICommand AddElement => new Command((s) => Members.Add(new ValCollectionEntry<T>(Members,
                                                                                _factory.MakeValElement(_entryAttribute) as ValElement<T>, ModifyVisibility)),
                                                           (s) => true);

        public override void Write(object instance)
        {
            // Collections will always have member info because they can't be standalone or part of a collection

            var ctor = _info.GetUnderlyingType().GetConstructor(new Type[] { });
            var collection = ctor.Invoke(null);
            var add = collection.GetType().GetMethod("Add");

            if (collection != null)
            {
                foreach (var member in Members)
                {
                    add.Invoke(collection, new object[] { member.GetValue() });
                }

                if (_info is PropertyInfo prop)
                {
                    prop.SetValue(instance, collection);
                }
                else if (_info is FieldInfo field)
                {
                    field.SetValue(instance, collection);
                }
                else
                {
                    throw new Exception("Attempting to write on an element without member info");
                }
            }
        }

        public override void Read(object instance)
        {
            ICollection<T> collection = null;

            if (_info is PropertyInfo prop)
            {
                collection = prop.GetValue(instance) as ICollection<T>;
            }
            else if (_info is FieldInfo field)
            {
                collection = field.GetValue(instance) as ICollection<T>;
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var newEle = _factory.MakeValElement(_entryAttribute) as ValElement<T>;
                    newEle.Value = item;
                    Members.Add(new ValCollectionEntry<T>(Members, newEle, ModifyVisibility));
                }
            }
        }
    }
}
