// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UiSon.Element;
using UiSon.Event;
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
        private IElement _element;
        private ElementManager _manager;
        private MemberInfo _identifingMember;
        private MemberInfo _info;

        /// <summary>
        /// Constructor
        /// </summary>
        public ElementSelectorVM(IElement element, ISelectorVM selector, MemberInfo identifingMember, MemberInfo info, ElementManager manager)
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
        }

        /// <summary>
        /// Reads data from instance and set's this editor's element's value to it
        /// </summary>
        public void Write(object instance)
        {
            if (_value != null)
            {
                if (_identifingMember != null)
                {
                    var element = _manager.Elements.FirstOrDefault(x => x.Name == _value);

                    // set value to value of identifying member
                    var hold = Activator.CreateInstance(_manager.ManagedType);

                    // lazy way of doing this, maybe make a more direct path to the value later
                    element.Write(hold);

                    if (_identifingMember is PropertyInfo prop)
                    {
                        _element.SetValue(prop.GetValue(hold));
                    }
                    else if (_identifingMember is FieldInfo field)
                    {
                        _element.SetValue(field.GetValue(hold));
                    }
                    else
                    {
                        throw new Exception("Attempting to read on an element without member info");
                    }
                }
                // if no identifying member, default to using the name of the element
                else
                {
                    _element.SetValue(_value);
                }
            }

            _selector.Write(instance);
        }

        public bool SetValue(object value)
        {
            var strValue = value?.ToString();

            // check to see if it's the name of an open element
            var element = _manager.Elements.FirstOrDefault(x => x.Name == strValue);

            if (element != null)
            {
                // Record the value here in the case of a ref ele so we know what to look up later
                _value = strValue;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(TextColor));
                return true;
            }
            else if (_selector.SetValue(value))
            {
                // clear out ref value so get defaults to the _selector instead
                _value = null;
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(TextColor));
                return true;
            }

            return false;
        }

        public object GetValueAs(Type type) => _selector.GetValueAs(type);

        public IEnumerable<DataGridColumn> GenerateColumns(string path) => _selector.GenerateColumns(path);

        public void Read(object instance)
        {
            if (_info == null)
            {
                _selector.Read(instance);
            }
            else
            {
                if (_info is PropertyInfo prop)
                {
                    _readValue = prop.GetValue(instance);
                }
                else if (_info is FieldInfo field)
                {
                    _readValue = field.GetValue(instance);
                }
            }
        }

        public void UpdateRefs()
        {
            // without an identifier, a name is recorded, makes things easy
            if (_identifingMember == null
                && _readValue is string readValueString
                && _manager.Elements.Any(x => x.Name == readValueString))
            {
                _value = readValueString;
            }
            // else find the name of the first manager that has the id we're looking for 
            else
            {
                // unfortunatly there is a loss of information in the saving process. Multiple items could share the same identifier and one identifier could reference another.
                // Finding just the first one with the same information makes it functionally the same on save, but could change the exact selection. Make a note of this in user docs.
                // That extra info could theoretically be saved in the project file, but i'd like to avaid that and allow the user to edit the jsons in any way they wish and not have to worry
                // about making sure they update the project file.
                _value = _manager.Elements.FirstOrDefault(x =>
                {
                    object id = null;

                    var hold = Activator.CreateInstance(_manager.ManagedType);

                    x.Write(hold);

                    if (_identifingMember is PropertyInfo prop)
                    {
                        id = prop.GetValue(hold);
                    }
                    else if (_identifingMember is FieldInfo field)
                    {
                        id = field.GetValue(hold);
                    }

                    // the string cast makes them comparable. Probably a better way...
                    return id?.ToString() == _readValue?.ToString();
                })?.Name;
            }

            OnPropertyChanged(nameof(Value));
        }

        private object _readValue;
    }
}
