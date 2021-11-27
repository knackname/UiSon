// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class UiSonElementAttribute : System.Attribute
    {
        public string Name { get; private set; }
        public string InitialValue { get; private set; }
        public string Extension { get; private set; }

        public UiSonElementAttribute(string name,
                                     string initialValue = null, string extension = ".json")
        {
            Name = name ?? string.Empty;
            InitialValue = initialValue;
            Extension = extension;
        }
    }
}
