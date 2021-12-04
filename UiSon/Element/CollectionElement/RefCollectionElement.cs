// UiSon, by Cameron Gale 2021

using System;
using System.Collections;
using System.Reflection;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Commands;
using UiSon.Extensions;

namespace UiSon.Element
{
    /// <summary>
    /// Several of the same element in a collection, able to be added or removed from
    /// </summary>
    public class RefCollectionElement : CollectionElement<RefCollectionEntry>
    {
        private ElementFactory _factory;

        private Type _entryType;
        private ConstructorInfo _entryConstructor;

        private MemberInfo _info;

        public RefCollectionElement(string name, int priority,
                                    DisplayMode displayMode, bool modifiable, CollectionType collectionType, Alignment alignment,
                                    Type entryType, MemberInfo info, ElementFactory factory)
            :base(name, priority, displayMode, modifiable, collectionType, alignment)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _info = info ?? throw new ArgumentNullException(nameof(info));

            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));
            _entryConstructor = _entryType.GetConstructor(new Type[] { });
        }

        public override ICommand AddElement => new Command((s) => Members.Add(new RefCollectionEntry(Members,
                                                                                                     _factory.MakeMainElement(_entryType),
                                                                                                     _entryConstructor, ModifyVisibility)), 
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
            IEnumerable collection = null;

            if (_info is PropertyInfo prop)
            {
                collection = prop.GetValue(instance) as IEnumerable;
            }
            else if (_info is FieldInfo field)
            {
                collection = field.GetValue(instance) as IEnumerable;
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var newEle = _factory.MakeMainElement(_entryType);
                    newEle.Read(item);
                    Members.Add(new RefCollectionEntry(Members, newEle, _entryConstructor, ModifyVisibility));
                }
            }
        }
    }
}
