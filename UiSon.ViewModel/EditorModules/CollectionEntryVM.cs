// UiSon, by Cameron Gale 2021

using System;
using System.Windows;
using System.Windows.Input;
using UiSon.Command;
using System.Collections.ObjectModel;
using UiSon.ViewModel.Interface;
using System.Collections.Generic;
using System.Windows.Controls;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates another <see cref="IEditorModule"/> to act as a collection entry
    /// </summary>
    public class CollectionEntryVM : NPCBase, ICollectionEntry
    {
        public string Name => null;

        public int Priority => 0;

        public IEditorModule Decorated => _decorated;
        private IEditorModule _decorated;

        public Visibility IsNameVisible => Visibility.Collapsed;

        public Visibility ModifyVisibility { get; private set; }

        private Collection<CollectionEntryVM> _parent;

        public CollectionEntryVM(Collection<CollectionEntryVM> parent, IEditorModule decorated, Visibility modifyVisibility)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            ModifyVisibility = modifyVisibility;

            _decorated.PropertyChanged += (s,b) => OnPropertyChanged(b.PropertyName);
        }

        public ICommand RemoveElement => new UiSonActionCommand((s) => _parent.Remove(this));

        public object GetValueAs(Type type) => _decorated.GetValueAs(type);

        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _decorated.GenerateColumns(path + $".{nameof(Decorated)}");

        public void Read(object instance) => _decorated.Read(instance);

        public void Write(object instance) => _decorated.Write(instance);

        public bool SetValue(object value) => _decorated.SetValue(value);

        public void UpdateRefs() => _decorated.UpdateRefs();
    }
}
