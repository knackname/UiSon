// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UiSon.Extension;

namespace UiSon.Element
{
    /// <summary>
    /// A string <see cref="IElement"/>
    /// </summary>
    public class StringElement : IElement
    {
        /// <summary>
        /// The element's value
        /// </summary>
        public string Value => _value;
        private string _value = null;

        /// <summary>
        /// If the element's value is nullable
        /// </summary>
        public bool IsNullable { get; private set; }

        private MemberInfo _info;
        private Type _memberType;
        private Type _valueType;
        private string _regexValidation;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="info">Member info for member this represents</param>
        /// <param name="regexValidation">validation string to scrub input</param>
        public StringElement(Type memberType, MemberInfo info = null, string regexValidation = null)
        {
            _memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));
            _info = info;
            _regexValidation = regexValidation;

            var utype = Nullable.GetUnderlyingType(_memberType);
            IsNullable = !_memberType.IsValueType || utype != null;
            _valueType = utype ?? _memberType;
        }

        /// <summary>
        /// Reads data from instance and set's this element's value to it
        /// </summary>
        /// <param name="instance">The instance</param>
        public void Read(object instance)
        {
            if (instance == null) { return; }

            if (_info is PropertyInfo prop)
            {
                SetValue(prop.GetValue(instance));
            }
            else if (_info is FieldInfo field)
            {
                SetValue(field.GetValue(instance));
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }
        }

        /// <summary>
        /// Writes this element's value to the instance
        /// </summary>
        /// <param name="instance">The instance</param>
        public void Write(object instance)
        {
            if (instance == null) { return; }

            if (_info is PropertyInfo prop)
            {
                prop.GetSetMethod(true).Invoke(instance, new[] { _value?.ParseAs(_memberType) });
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, _value?.ParseAs(_memberType));
            }
            else
            {
                throw new Exception("Attempting to write on an element without member info");
            }
        }

        /// <summary>
        /// Attempts to set the value to the input. The string "null" is a keyword that will
        /// set the value to null.
        /// </summary>
        /// <param name="input">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        public bool SetValue(object input)
        {
            // null
            if (input == null)
            {
                if (_memberType.IsValueType)
                {
                    return false;
                }
                else
                {
                    _value = null;
                    return true;
                }
            }

            // special keyword and regex handling for strings
            if (input is string inputString)
            {
                // null is a keyword that sets the value to null
                if (inputString == "null" && !_memberType.IsValueType)
                {
                    _value = null;
                    return true;
                }
                // validate input with regex, reject those failing
                else if (!(_regexValidation == null || Regex.IsMatch(inputString, _regexValidation)))
                {
                    return false;
                }
            }

            // attempt to parse other types 
            var asString = input.ToString();
            if (asString?.ParseAs(_valueType) != null)
            {
                _value = asString;
                return true;
            }

            return false;
        }

        public object GetValueAs(Type type) => _value.ParseAs(type);

        private struct poopy
        {
            public string Key => _key;
            private string _key;
        }

    }
}
