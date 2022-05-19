// UiSon, by Cameron Gale 2021

using System.Text.RegularExpressions;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A string <see cref="IUiSonElement"/>
    /// </summary>
    public class StringView : NPCBase, IReadWriteView
    {
        /// <inheritdoc/>
        public object? Value => _value;
        private string? _value = null;

        private readonly bool _isNullable;
        private readonly ValueMemberInfo? _info;
        private readonly Type _memberType;
        private readonly Type _valueType;
        private readonly string? _regexValidation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="memberType"></param>
        /// <param name="info"></param>
        /// <param name="regexValidation"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public StringView(Type memberType, ValueMemberInfo? info = null, string? regexValidation = null)
        {
            _memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));

            _info = info;
            _regexValidation = regexValidation;

            var utype = Nullable.GetUnderlyingType(_memberType);
            _isNullable = !_memberType.IsValueType || utype != null;
            _valueType = utype ?? _memberType;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? input)
        {
            // null
            if (input == null)
            {
                if (_isNullable)
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

            // special keyword and regex handling for strings
            if (input is string inputString)
            {
                // null is a keyword that sets the value to null
                if (inputString == "null" && _isNullable)
                {
                    _value = null;
                    OnPropertyChanged(nameof(Value));
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
            if (asString.ParseAs(_valueType) != null)
            {
                _value = asString;
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value) => TrySetValue(value);

        /// <inheritdoc/>
        public void Read(object instance) => TrySetValue(_info.GetValue(instance));

        /// <inheritdoc/>
        public void Write(object instance) => _info?.SetValue(instance, _value.ParseAs(_memberType));
    }
}
