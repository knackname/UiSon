// UiSon, by Cameron Gale 2021

using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using UiSon.Commands;
using UiSon.Element.Element.Interface;

namespace UiSon.Element
{
    /// <summary>
    /// Decorates an element to add removal funtionality
    /// </summary>
    public class RefCollectionEntry : ICollectionEntry
    {
        public Visibility ModifyVisibility { get; private set; }

        public IElement Element { get; private set; }

        private Collection<RefCollectionEntry> _parent;
        private ConstructorInfo _memberConstructor;

        public RefCollectionEntry(Collection<RefCollectionEntry> parent,
                                  IElement element, ConstructorInfo memberConstructor, Visibility modifyVisibility)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Element = element ?? throw new ArgumentNullException(nameof(element));
            _memberConstructor = memberConstructor ?? throw new ArgumentNullException(nameof(memberConstructor));
            ModifyVisibility = modifyVisibility;
        }

        public ICommand RemoveElement => new Command((s) => _parent.Remove(this), (s) => true);

        public object GetValue()
        {
            var value = _memberConstructor.Invoke(null); 

            Element.Write(value);

            return value;
        }
    }
}
