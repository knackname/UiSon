// UiSon, by Cameron Gale 2021

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UiSon.Commands;
using UiSon.Element.Element.Interface;

namespace UiSon.Element
{
    public class ValCollectionEntry<T> : ICollectionEntry
    {
        public Visibility ModifyVisibility { get; private set; }

        public IElement Element => _element;
        private ValElement<T> _element;

        private Collection<ValCollectionEntry<T>> _parent;

        public ValCollectionEntry(Collection<ValCollectionEntry<T>> parent, ValElement<T> element, Visibility modifyVisibility)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _element = element ?? throw new ArgumentNullException(nameof(element));
            ModifyVisibility = modifyVisibility;
        }

        public ICommand RemoveElement => new Command((s) => _parent.Remove(this), (s) => true);

        public T GetValue() => _element.Value;
    }
}
