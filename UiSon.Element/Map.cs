// UiSon, by Cameron Gale 2022

using System.Collections.Generic;

namespace UiSon.Element
{
    /// <summary>
    /// Data structure with two way lookup
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Map<T1, T2>
    {
        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }

        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 key, T2 value)
        {
            _forward.Add(key, value);
            _reverse.Add(value, key);
        }

        public IEnumerable<T1> Keys => _forward.Keys;

        public IEnumerable<T2> Values => _forward.Values;
    }
}
