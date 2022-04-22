﻿// UiSon, by Cameron Gale 2022

using System;
using UiSon.Extension;

namespace UiSon.Element
{
    /// <summary>
    /// A value type <see cref="IUiSonElement"/>
    /// </summary>
    /// <typeparam name="T">a Value type</typeparam>
    public class ValueElement<T> : NPCBase, IUiSonElement
        where T : struct
    {
        /// <summary>
        /// The element's value
        /// </summary>
        public T? Value => _value;
        protected T? _value = null;

        /// <summary>
        /// If the element's value is nullable
        /// </summary>
        public bool IsNullable { get; private set; }

        protected ValueMemberInfo _info;
        private Type _memberType;
        protected Type _valueType;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="memberType">The type this element writes to</param>
        /// <param name="info">Member info for member this represents</param>
        public ValueElement(Type memberType, ValueMemberInfo info = null)
        {
            _memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
            _info = info;

            var utype = Nullable.GetUnderlyingType(_memberType);
            IsNullable = !_memberType.IsValueType || utype != null;
            _valueType = utype ?? _memberType;
        }

        /// <summary>
        /// Reads data from instance and set's this element's value to it
        /// </summary>
        /// <param name="instance">The instance</param>
        public void Read(object instance) => SetValue(_info.GetValue(instance));

        /// <summary>
        /// Writes this element's value to the instance
        /// </summary>
        /// <param name="instance">The instance</param>
        public void Write(object instance)
        {
            object casted = null;
            _value?.TryCast(_valueType, out casted);
            _info.SetValue(instance, casted);
        }

        /// <summary>
        /// Returns the element's value as the given type
        /// </summary>
        /// <param name="type">The type to retreieve the value as</param>
        /// <returns>The element's value as the type</returns>
        public virtual object GetValueAs(Type type)
        {
            if (_value?.TryCast(type, out var cast) ?? false)
            {
                return cast;
            }

            return null;
        }

        /// <summary>
        /// Attempts to set the value to the input.
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        public virtual bool SetValue(object input)
        {
            // null
            if (input == null)
            {
                if (IsNullable)
                {
                    _value = null;
                    OnPropertyChanged(nameof(Value));
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // make sure value can be set from the element's type
            else if (input.TryCast(typeof(T), out var asT)
                     && asT.TryCast(_valueType, out var _))
            {
                _value = (T)asT;
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }
    }
}
