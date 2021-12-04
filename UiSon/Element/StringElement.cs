﻿// UiSon, by Cameron Gale 2021

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Media;
using UiSon.Extensions;

namespace UiSon.Element
{
    public abstract class StringElement : ValElement<string>
    {
        private static readonly Color Red = new Color() { R = 255, G = 100, B = 100, A = 255 };
        private static readonly Color Black = new Color() { R = 0, G = 0, B = 0, A = 255 };

        public Brush TextColor => new SolidColorBrush(Value == "null" ? Red : Black) ;

        public override string Value
        {
            get => _value ?? "null";
            set
            {
                if (value == "null")
                {
                    value = null;
                }

                if (_value != value
                    &&
                    (_regexValidation == null
                     || Regex.IsMatch(value, _regexValidation)))
                {
                    _value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }
        private string _value;

        private string _regexValidation;

        public StringElement(string name, int priority, MemberInfo info, string regexValidation = null)
            :base(name, priority, info)
        {
            _regexValidation = regexValidation;
        }

        public override void Write(object instance)
        {
            var memberType = _info.GetUnderlyingType();
            object convertedValue = null;

            if (memberType == typeof(bool)
                && bool.TryParse(Value, out var asBool))
            {
                convertedValue = asBool;
            }
            else if(memberType == typeof(char)
                && char.TryParse(Value, out var asChar))
            {
                convertedValue = asChar;
            }
            else if(memberType == typeof(int)
                && int.TryParse(Value, out var asInt))
            {
                convertedValue = asInt;
            }
            else if(memberType == typeof(float)
                && float.TryParse(Value, out var asFloat))
            {
                convertedValue = asFloat;
            }
            else if (memberType == typeof(double)
                && float.TryParse(Value, out var asDouble))
            {
                convertedValue = asDouble;
            }
            else if (memberType == typeof(string))
            {
                convertedValue = Value.ToString();
            }

            if (_info is PropertyInfo prop)
            {
                prop.SetValue(instance, convertedValue);
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, convertedValue);
            }
            else
            {
                throw new Exception("Attempting to write on an element without member info");
            }
        }

        public override void Read(object instance)
        {
            object instanceValue = null;

            if (_info is PropertyInfo prop)
            {
                instanceValue = prop.GetValue(instance);
            }
            else if (_info is FieldInfo field)
            {
                instanceValue = field.GetValue(instance);
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }

            if (instanceValue is bool boolValue)
            {
                Value = boolValue.ToString();
            }
            else if (instanceValue is char charValue)
            {
                Value = charValue.ToString();
            }
            else if (instanceValue is int intValue)
            {
                Value = intValue.ToString();
            }
            else if (instanceValue is float floatValue)
            {
                Value = floatValue.ToString();
            }
            else if (instanceValue is double doubleValue)
            {
                Value = doubleValue.ToString();
            }
            else if (instanceValue is string stringValue)
            {
                Value = stringValue;
            }
        }
    }
}