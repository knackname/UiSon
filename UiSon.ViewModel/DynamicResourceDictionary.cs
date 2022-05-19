// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Windows;
using UiSon.Extension;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Dictionary containing multiple sources, capable of switching between them
    /// </summary>
    public class DynamicResourceDictionary : ResourceDictionary
    {
        public IEnumerable<string> Keys => sources.Keys;

        public string Current { get; private set; }

        private readonly Dictionary<string,Uri> sources = new Dictionary<string, Uri>();

        public void AddSource(string name, Uri uri)
        {
             sources.AddOrReplace(name, uri);

            if (base.Source == null)
            {
                base.Source = uri;
                Current = name;
            }
        }

        /// <summary>
        /// Changes the source to the one of the given name
        /// </summary>
        /// <param name="name">Name of the source</param>
        public void ChangeSource(string name)
        {
            if (sources.ContainsKey(name))
            {
                base.Source = sources[name];
                Current = name;
            }
        }
    }
}
