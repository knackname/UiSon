// UiSon 2021, Cameron Gale

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace UiSon
{
    /// <summary>
    /// Collection that notifies when a item contained withing calls a property changed event
    /// </summary>
    /// <typeparam name="T">item type, must impliment INotifyPropertyChanged</typeparam>
    public class NotifyingCollection<T> : ICollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Called when an item is added, removes or the collection is cleared
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Called when an item contained calls its PropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The decorated observable collection
        /// </summary>
        private ObservableCollection<T> _collection = new ObservableCollection<T>();

        /// <summary>
        /// Reutrns an IEnumerator for the collection
        /// </summary>
        /// <returns>An IEnumerator for the collection</returns>
        IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

        /// <summary>
        /// Returns the number of elements in the collection
        /// </summary>
        public int Count => _collection.Count;

        /// <summary>
        /// The collection is never read only; always returns false. Implimented for ICollection
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Constructor
        /// </summary>
        public NotifyingCollection()
        {
            _collection.CollectionChanged += (s, e) => CollectionChanged.Invoke(s,e);
        }

        /// <summary>
        /// Adds an item to the collection
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(T item)
        {
            item.PropertyChanged += OnItemPropertyChanged;
            _collection.Add(item);
        }

        /// <summary>
        /// Removes all items from the collection
        /// </summary>
        public void Clear()
        {
            foreach(var item in _collection)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
            }
            _collection.Clear();
        }

        /// <summary>
        /// Reutrns wether the item is contained in the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if the item in contained</returns>
        public bool Contains(T item) => _collection.Contains(item);

        /// <summary>
        /// Copies the entire collection to a compatiable 1d array, starting at the specified index
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Starting index</param>
        public void CopyTo(T[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The item to remove</returns>
        public bool Remove(T item)
        {
            item.PropertyChanged -= OnItemPropertyChanged;
            return _collection.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator for the collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

        /// <summary>
        /// Passes on the property changed event from items
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        private void OnItemPropertyChanged(object source, PropertyChangedEventArgs args) => PropertyChanged.Invoke(source, args);
    }
}
