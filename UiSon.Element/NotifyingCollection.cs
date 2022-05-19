// UiSon 2021, Cameron Gale

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace UiSon.Element
{
    /// <summary>
    /// Collection that notifies when a item contained within calls a property changed event.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    public class NotifyingCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Called when an item is added, removed or the collection is cleared.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

        /// <summary>
        /// Called when an item contained calls its <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        /// <inheritdoc cref="Collection{T}.Count" />
        public int Count => _collection.Count;

        /// <inheritdoc cref="ICollection{T}.IsReadOnly" />
        public bool IsReadOnly => false;

        private readonly ObservableCollection<T> _collection = new ObservableCollection<T>();

        /// <summary>
        /// Constructor
        /// </summary>
        public NotifyingCollection()
        {
            _collection.CollectionChanged += (s, e) => CollectionChanged.Invoke(s,e);
        }

        /// <summary>
        /// Passes on the property changed event from items
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="args">The arguments.</param>
        private void OnItemPropertyChanged(object source, PropertyChangedEventArgs args) => PropertyChanged.Invoke(source, args);

        /// <inheritdoc cref="ICollection{T}.Add(T)" />
        public void Add(T item)
        {
            item.PropertyChanged += OnItemPropertyChanged;
            _collection.Add(item);
        }

        /// <inheritdoc cref="ICollection{T}.Remove(T)" />
        public bool Remove(T item)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
            return _collection.Remove(item);
        }

        /// <inheritdoc cref="ICollection{T}.Clear" />
        public void Clear()
        {
            foreach(var item in _collection)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
            _collection.Clear();
        }

        /// <inheritdoc cref="ICollection{T}.Contains(T)" />
        public bool Contains(T item) => _collection.Contains(item);

        /// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)" />
        public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
    }
}
