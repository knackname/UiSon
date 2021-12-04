// UiSon 2021, Cameron Gale

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using UiSon.Events;

namespace UiSon
{
    public class NotifyingCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<T> _collection = new ObservableCollection<T>();

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        public int Count => _collection.Count;

        public bool IsReadOnly => false;

        public NotifyingCollection()
        {
            _collection.CollectionChanged += (s, e) => CollectionChanged.Invoke(s,e);
        }

        public void Add(T item)
        {
            item.PropertyChanged += OnItemPropertyChanged;
            _collection.Add(item);
        }

        public void Clear()
        {
            foreach(var item in _collection)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
            _collection.Clear();
        }

        public bool Contains(T item) => _collection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
            return _collection.Remove(item);
        }

        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        private void OnItemPropertyChanged(object source, PropertyChangedEventArgs args) => PropertyChanged.Invoke(source, args);
    }
}
