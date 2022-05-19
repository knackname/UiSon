// UiSon, by Cameron Gale 2022

using System.Collections.Generic;

namespace UiSon.Element
{
    /// <summary>
    /// Data structure with two way lookup
    /// </summary>
    /// <typeparam name="T1">The first value's type.</typeparam>
    /// <typeparam name="T2">The second value's type.</typeparam>
    public class Map<T1, T2>
    {
        /// <summary>
        /// Indexer to lookup the second value using the first.
        /// </summary>
        public Indexer<T1, T2> FirstToSecond { get; private set; }
        private readonly Dictionary<T1, T2> _firstToSecond = new Dictionary<T1, T2>();

        /// <summary>
        /// Indexer to lookup the first value using the second.
        /// </summary>
        public Indexer<T2, T1> SecondToFirst { get; private set; }
        private readonly Dictionary<T2, T1> _secondToFirst = new Dictionary<T2, T1>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Map()
        {
            FirstToSecond = new Indexer<T1, T2>(_firstToSecond);
            SecondToFirst = new Indexer<T2, T1>(_secondToFirst);
        }

        /// <summary>
        /// Adds the specified values to the map.
        /// </summary>
        /// <param name="firstValue">The first value.</param>
        /// <param name="secondValue">The second value.</param>
        public void Add(T1 firstValue, T2 secondValue)
        {
            _firstToSecond.Add(firstValue, secondValue);
            _secondToFirst.Add(secondValue, firstValue);
        }

        /// <summary>
        /// Returns an enumerable containing all the first values in this map. 
        /// </summary>
        public IEnumerable<T1> FirstValues => _firstToSecond.Keys;

        /// <summary>
        /// Returns an enumerable containing all the second values in this map. 
        /// </summary>
        public IEnumerable<T2> SecondValues => _firstToSecond.Values;
    }
}
