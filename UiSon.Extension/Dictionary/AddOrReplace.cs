// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;

namespace UiSon.Extension
{
    public static partial class ExtendDictionary
    {
        /// <summary>
        /// If the dictonary already contains the key, the value associated with that key is replaced with the value.
        /// Otherwise, the key value pair is added to the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null) { throw new ArgumentNullException(nameof(dictionary)); }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
