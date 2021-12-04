// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using UiSon.Extensions;

namespace UiSon.Element
{
    /// <summary>
    /// A checkbox with a label
    /// </summary>
    public class CheckboxElement : ValElement<bool>
    {
        public override bool Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _value;

        public CheckboxElement(string name, int priority, bool initialValue, MemberInfo info)
            :base(name, priority, info)
        {
            _value = initialValue;
        }

        public override IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridCheckBoxColumn();
            valCol.Header = Name;

            var valBinding = new Binding(path + ".Value");
            valBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            valCol.Binding = valBinding;

            return new List<DataGridColumn>() { valCol };
        }

        public override void Write(object instance)
        {
            var memberType = _info.GetUnderlyingType();
            object convertedValue = null;

            if (memberType == typeof(bool))
            {
                convertedValue = Value;
            }
            else if (memberType == typeof(char))
            {
                convertedValue = Value ? 'T':'F';
            }
            else if (memberType == typeof(int)
                || memberType == typeof(float)
                || memberType == typeof(double))
            {
                convertedValue = Value ? 1 : 0;
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
                Value = boolValue;
            }
            else if (instanceValue is char charValue)
            {
                Value = charValue == 'T';
            }
            else if (instanceValue is int intValue)
            {
                Value = intValue > 0;
            }
            else if (instanceValue is float floatValue)
            {
                Value = floatValue > 0;
            }
            else if (instanceValue is double doubleValue)
            {
                Value = doubleValue > 0;
            }
            else if (instanceValue is string stringValue)
            {
                if (bool.TryParse(stringValue, out var asBool))
                {
                    Value = asBool;
                }
                else
                {
                    Value = false;
                }
            }
        }
    }
}
