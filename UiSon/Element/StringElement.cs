// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UiSon.Element
{
    public abstract class StringElement : ValElement<string>
    {
        public override string Value
        {
            get => _value ?? "null";
            set
            {
                if (_value != value
                    &&
                    (_regexValidation == null
                     || Regex.IsMatch(value, _regexValidation)))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _value;

        private MemberInfo _info;
        private string _regexValidation;

        public StringElement(string name, int priority, MemberInfo info, string regexValidation = null)
            :base(name, priority)
        {
            _info = info;
            _regexValidation = regexValidation;
        }

        public override void Write(object instance)
        {
            var value = Value == "null" ? null : Value;

            if (_info is PropertyInfo prop)
            {
                prop.SetValue(instance, value);
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
            else
            {
                throw new Exception("Attempting to write on an element without member info");
            }
        }

        public override void Read(object instance)
        {
            if (_info is PropertyInfo prop)
            {
                Value = (string)prop.GetValue(instance);
            }
            else if (_info is FieldInfo field)
            {
                Value = (string)field.GetValue(instance);
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }
        }
    }
}
