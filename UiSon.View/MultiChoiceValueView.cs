// UiSon, by Cameron Gale 2022

using System.Collections;
using System.ComponentModel;
using System.Reflection;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    // Is a collecting of several checkboxes, then to get value adds what those checkboxes represent to a collection
    public class MultiChoiceValueView : GroupView, IEncapsulatingView
    {
        /// <inheritdoc/>
        public object? Value
        {
            get
            {
                if (_type.IsArray)
                {
                    var entries = new List<object?>();

                    foreach (var member in _members)
                    {
                        if (member.IsChecked)
                        {
                           entries.Add(member.TrueValue);
                        }
                    }

                    var value = Array.CreateInstance(_entryType, entries.Count) as IList;

                    int index = 0;
                    foreach (var entry in entries)
                    {
                        value[index] = entry;
                        index++;
                    }

                    return value;
                }
                else
                {
                    var value = _type.GetDefaultValue();

                    // some implimentations of ICollection<> may have rules about what can be added and throw an error
                    try
                    {
                        foreach (var member in _members)
                        {
                            if (member.IsChecked)
                            {
                                _collectionAdd.Invoke(value, new[] { member.TrueValue });
                            }
                        }

                        _state = ModuleState.Normal;
                        OnPropertyChanged(nameof(State));
                        return value;
                    }
                    catch (Exception e)
                    {
                        _state = ModuleState.Error;
                        _stateJustification = $"Exception throw when adding elements to the collection - {e}";
                        OnPropertyChanged(nameof(State));
                        return null;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public UiType UiType => UiType.Encapsulating;

        /// <inheritdoc/>
        public object? DisplayValue => Value;

        /// <inheritdoc/>
        public Type? Type => _type;
        private readonly Type _type;

        /// <inheritdoc/>
        public override ModuleState State => _state;
        private ModuleState _state;

        /// <inheritdoc/>
        public override string StateJustification => _stateJustification ?? base.StateJustification;
        private string? _stateJustification = string.Empty;

        private readonly Type _entryType;
        private readonly MethodInfo _collectionAdd;
        private readonly MultiChoiceOptionView[] _members;
        private readonly ValueMemberInfo? _info;

        public MultiChoiceValueView(Type type,
                                    Type entryType,
                                    int displayPriority,
                                    string? name,
                                    DisplayMode displayMode,
                                    ValueMemberInfo? info,
                                    MultiChoiceOptionView[] members)
            : base(displayPriority, name, displayMode, members)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));
            _info = info ?? throw new ArgumentNullException(nameof(info));

            var collectionAdd = type.GetInterfaces()
                      .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                      .GetMethod("Add");

            _collectionAdd = collectionAdd ?? throw new ArgumentException(nameof(type));
            _members = members ?? throw new ArgumentException(nameof(members));
        }

        /// <inheritdoc/>
        public override void SetValue(object? value)
        {
            if (value is IEnumerable enumerable
                && value.GetType()
                        .GetInterfaces()
                        .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                        .GetGenericArguments()
                        .FirstOrDefault() == _entryType)
            {
                foreach (var member in _members)
                {
                    member.SetValue(false);
                }

                foreach (var entry in enumerable)
                {
                    var member = _members.FirstOrDefault(x => x.Name == entry.ToString());

                    if (member != null)
                    {
                        member.SetValue(true);
                    }
                }

                _state = ModuleState.Normal;
                _stateJustification = null;

                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(State));
            }
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            if (value is IEnumerable enumerable
                && value.GetType()
                        .GetInterfaces()
                        .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                        .GetGenericArguments()
                        .FirstOrDefault() == _entryType)
            {
                foreach (var member in _members)
                {
                    member.SetValue(false);
                }

                foreach (var entry in enumerable)
                {
                    var member = _members.FirstOrDefault(x => x.Name == entry.ToString());

                    if (member != null)
                    {
                        member.SetValue(true);
                    }
                }

                _state = ModuleState.Normal;
                _stateJustification = null;

                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(State));

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override void Read(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Read called on view without member info");
            }

            SetValueFromRead(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public override void Write(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Write called on view without member info");
            }

            if (_state != ModuleState.Error)
            {
                _info.SetValue(instance, Value);
            }
        }
    }

    public class MultiChoiceOptionView : NPCBase, IUiValueView
    {
        /// <inheritdoc/>
        public UiType UiType => UiType.Checkbox;

        /// <inheritdoc/>
        public object? Value => _isChecked;
        public bool IsChecked => _isChecked;
        private bool _isChecked;

        /// <inheritdoc/>
        public object? DisplayValue => Value;

        /// <inheritdoc/>
        public Type? Type => typeof(bool);

        /// <inheritdoc/>
        public int DisplayPriority => 0;

        /// <inheritdoc/>
        public ModuleState State => ModuleState.Normal;

        /// <inheritdoc/>
        public string StateJustification => string.Empty;

        /// <inheritdoc/>
        public string? Name => _name;
        private readonly string _name;

        /// <summary>
        /// Value to be written
        /// </summary>
        public object? TrueValue => _trueValue;
        private readonly object? _trueValue;

        public MultiChoiceOptionView(object? value, string name)
        {
            _trueValue = value;
            _name = name;
        }

        /// <inheritdoc/>
        public void SetValue(object? value)
        {
            if (value is bool asBool)
            {
                _isChecked = asBool;
            }
            else
            {
                _isChecked = false;
            }

            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(DisplayValue));
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SetValueFromRead(object? value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Read(object instance)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            throw new NotImplementedException();
        }
    }
}
