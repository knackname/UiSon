using System.Collections.Generic;

namespace UiSon.Extension
{
    public static partial class ExtendDictionary
    {
        public static void Add<T, U>(this Dictionary<T, U> dict, KeyValuePair<T, U> kvp) => dict.Add(kvp.Key, kvp.Value);
    }
}
