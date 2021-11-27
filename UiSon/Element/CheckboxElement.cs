// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

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

        private MemberInfo _info;

        public CheckboxElement(string name, int priority, bool initialValue, MemberInfo info)
            :base(name, priority)
        {
            _value = initialValue;
            _info = info;
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
            if (_info is PropertyInfo prop)
            {
                prop.SetValue(instance, Value);
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, Value);
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
                Value = (bool)prop.GetValue(instance);
            }
            else if (_info is FieldInfo field)
            {
                Value = (bool)field.GetValue(instance);
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }
        }
    }
}
