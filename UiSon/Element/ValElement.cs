// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;

namespace UiSon.Element
{
    /// <summary>
    /// An element representing a value type, or a string
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ValElement<T> : BaseElement
    {
        public abstract T Value { get; set; }
        internal MemberInfo _info;

        public ValElement(string name, int priority, MemberInfo info)
            :base(name, priority)
        {
            _info = info;
        }
    }
}
