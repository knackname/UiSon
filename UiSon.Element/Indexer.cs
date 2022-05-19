// UiSon, by Cameron Gale 2022

using System.Collections.Generic;

namespace UiSon.Element
{
    /// <summary>
    /// Wraps a dicionary to only allow access to entry lookup.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class Indexer<TKey, TValue>
    {
        /// <summary>
        /// The wrapped <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        private Dictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dictionary">The wrapped <see cref="Dictionary{TKey, TValue}"/>.</param>
        public Indexer(Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// Gets or sets the value associated with the specific key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value associated with the key.</returns>
        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
            set { _dictionary[key] = value; }
        }
    }
}
