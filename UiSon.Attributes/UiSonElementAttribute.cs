// UiSon, by Cameron Gale 2021

using System;

namespace UiSon.Attribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class UiSonElementAttribute : System.Attribute
    {
        public bool IncludeFields { get; private set; }
        public string InitialValue { get; private set; }
        public string Extension { get; private set; }

        /// <summary>
        /// Attribute designating a class or struct as a UiSonElement. Json files representing an instance of this
        /// will be creatable in UiSon
        /// </summary>
        /// <param name="extension">The file extension for json files made from this</param>
        /// <param name="initialValue">A json string representing the instance used for new eleemnts of this type</param>
        public UiSonElementAttribute(bool includeFields = true, string extension = ".json", string initialValue = null)
        {
            IncludeFields = includeFields;
            InitialValue = initialValue;
            Extension = extension;
        }
    }
}
