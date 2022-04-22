// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UiSon.Element;
using UiSon.Event;
using UiSon.Extension;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Decorates a selector to select from classes
    /// </summary>
    public class ElementSelectorVM : NPCBase, IEditorModule, ISelectorVM
    {
        public string Name => _selector.Name;

        public int Priority => _selector.Priority;

        public string Value
        {
            get => _value ?? _selector.Value;
            set => SetValue(value);
        }
        private string _value;

        public IEnumerable<string> Options => _selector.Options.Concat(_manager.Elements.Select(x => x.Name)).Distinct();

        public Brush TextColor => _value == null ? _selector.TextColor : UiSonColors.Black;

        public Visibility IsNameVisible => _selector.IsNameVisible;

        private ISelectorVM _selector;
        private IUiSonElement _element;
        private ElementManager _manager;
        private ValueMemberInfo _identifingMember;
        private ValueMemberInfo _info;

        /// <summary>
        /// Constructor
        /// </summary>
        public ElementSelectorVM(IUiSonElement element, ISelectorVM selector, ValueMemberInfo identifingMember, ValueMemberInfo info, ElementManager manager)
        {
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _info = info;
            _identifingMember = identifingMember;

            manager.Elements.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "Name"
                    && e is PropertyChangedExtendedEventArgs<string> extended)
                {
                    if (_value == extended.OldValue)
                    {
                        OnPropertyChanged(nameof(Options));
                        _value = extended.NewValue;
                        OnPropertyChanged(nameof(Value));
                        OnPropertyChanged(nameof(TextColor));
                    }
                    else
                    {
                        OnPropertyChanged(nameof(Options));
                    }
                }
            };
            manager.Elements.CollectionChanged += (s, e) => OnPropertyChanged(nameof(Options));

            _element.PropertyChanged += Refresh;
        }

        public bool SetValue(object value)
        {
            var strValue = value as string;

            // see if it's the name of an element
            var element = _manager.Elements.FirstOrDefault(x => x.Name == strValue);
            if (element != null)
            {
                _value = strValue;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(TextColor));
                return true;
            }
            // otherwise try to have the decorated selector take care of it
            else if (_selector.SetValue(value))
            {
                //if succesfull clear all the the ele selector data so the _selector is used
                _value = null;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(TextColor));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads data from instance and set's this editor's element's value to it
        /// Reading for this module won't update the direct value. UpdateRefs must also be called after it's
        /// dependant modules are updated.
        /// </summary>
        public void Read(object instance)
        {
            _readValue = _info.GetValue(instance);
        }

        private object _readValue;

        /// <summary>
        /// Writes this editor's element's value to the instance
        /// </summary>
        public void Write(object instance)
        {
            if (_value == null)
            {
                _selector.Write(instance);
            }
            else
            {
                var value = _identifingMember == null 
                    ? _value 
                    : GetElementValue(_manager.Elements.FirstOrDefault(x => x.Name == _value));

                _info.SetValue(instance, value);
            }
        }

        private object GetElementValue(ElementVM element)
        {
            if (_identifingMember == null)
            {
                return _value;
            }

            // set value to value of identifying member
            var hold = Activator.CreateInstance(_manager.ManagedType);

            // lazy way of doing this, maybe make a more direct path to the value later
            element.Write(hold);

            return _identifingMember.GetValue(hold);
        }

        public void UpdateRefs()
        {
            if (_identifingMember == null)
            {
                // without an identifier, a name is recorded, makes things easy
                if (_readValue is string readValueString
                    && _manager.Elements.Any(x => x.Name == readValueString))
                {
                    Value = readValueString;
                }
            }
            // else find the name of the first manager that has the id we're looking for 
            else
            {
                // unfortunatly there is a loss of information in the saving process. Multiple items could share the same identifier and one identifier could reference another.
                // Finding just the first one with the same information makes it functionally the same on save, but could change the exact selection. Make a note of this in user docs.
                // That extra info could theoretically be saved in the project file, but i'd like to avaid that and allow the user to edit the jsons in any way they wish and not have to worry
                // about making sure they update the project file.
                Value = _manager.Elements.FirstOrDefault(x =>
                {
                    var hold = Activator.CreateInstance(_manager.ManagedType);

                    x.Write(hold);

                    // the string cast makes them comparable. Probably a better way...
                    return _identifingMember.GetValue(hold)?.ToString() == _readValue?.ToString();
                })?.Name;
            }
        }

        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _selector.GenerateColumns(path);

        public object GetValueAs(Type type)
        { 
            if (_value == null)
            {
                return _selector.GetValueAs(type);
            }

            var value = GetElementValue(_manager.Elements.FirstOrDefault(x => x.Name == _value));

            if (value is string asStr)
            {
                return asStr.ParseAs(type);
            }

            if (value.TryCast(type, out object cast))
            {
                return cast;
            }

            return null;
        }

        private void Refresh(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(IsNameVisible));
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(Options));
        }
    }
}
