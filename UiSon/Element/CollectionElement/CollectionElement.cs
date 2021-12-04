// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Element.Element.Interface;

namespace UiSon.Element
{
    public abstract class CollectionElement<T> : BaseElement, ICollectionElement
    {
        private bool _modifiable;
        public CollectionType Type { get; private set; }
        public DisplayMode DisplayMode { get; private set; }
        public HorizontalAlignment HorizontalAlignment { get; private set; }
        public Visibility ModifyVisibility => _modifiable ? Visibility.Visible : Visibility.Collapsed;
        public ObservableCollection<T> Members => _members;
        private ObservableCollection<T> _members = new ObservableCollection<T>();

        public CollectionElement(string name, int priority,
                                 DisplayMode displayMode, bool modifiable, CollectionType collectionType, Alignment alignment)
            :base(name, priority)
        {
            _modifiable = modifiable;
            DisplayMode = displayMode;
            Type = collectionType;

            switch (alignment)
            {
                case Alignment.Left:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case Alignment.Center:
                    HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case Alignment.Right:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case Alignment.Stretch:
                    HorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
            }
        }

        public abstract ICommand AddElement { get; }

        // This is only for collections, and there cannot be a collection of collections due to the way attributes work
        public override IEnumerable<DataGridColumn> GenerateColumns(string path) => throw new NotImplementedException();
    }
}
