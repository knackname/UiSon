// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class ElementSelectorView : NPCBase, ISelectorView
    {
        /// <inheritdoc/>
        public object? Value
        {
            get => _value ?? _decorated.Value;
            set => TrySetValue(value);
        }
        private string? _value;

        /// <inheritdoc/>
        public IEnumerable<string> Options => _decorated.Options.Concat(_elementManager.Elements.Select(x => x.Name)).Distinct();

        private readonly ISelectorView _decorated;
        private readonly IElementManager _elementManager;
        private readonly ValueMemberInfo? _identifingMember;
        private readonly ValueMemberInfo? _info;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="decorated">The decorated view</param>
        /// <param name="identifingMember"></param>
        /// <param name="info"></param>
        /// <param name="elementManager"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ElementSelectorView(ISelectorView decorated, ValueMemberInfo? identifingMember, ValueMemberInfo? info, IElementManager elementManager)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _identifingMember = identifingMember ?? throw new ArgumentNullException(nameof(identifingMember));
            _elementManager = elementManager ?? throw new ArgumentNullException(nameof(elementManager));

            _info = info;
        }

        /// <inheritdoc/>
        public void Read(object instance) => TrySetValueFromRead(instance);

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            if (_info == null) { throw new Exception("Read called on view without value member info."); }

            // unfortunatly there is a loss of information in the saving process.
            // Multiple items could share the same identifier and one identifier could reference another.
            // Finding just the first one with the same information makes it functionally the same on save,
            // but could change the exact selection. Make a note of this in user docs.
            // That extra info could be saved in the project file, but I'd like to avaid that
            // and allow the user to edit the jsons in any way they wish and not have to worry
            // about making sure they update the project file.

            var readValue = _info.GetValue(value).ToString();

            var found = _identifingMember == null
                ? _elementManager.Elements.FirstOrDefault(x => x.Name == readValue)?.Name
                : _elementManager.Elements.FirstOrDefault(x => _identifingMember.GetValue(x.Value)?.ToString() == readValue)?.Name;

            if (found != null)
            {
                _value = found;
                OnPropertyChanged(nameof(Value));
                return true;
            }
            else if (_decorated.TrySetValueFromRead(value))
            {
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            var strValue = value as string;

            // see if it's the name of an element
            var element = _elementManager.Elements.FirstOrDefault(x => x.Name == strValue);
            if (element != null)
            {
                _value = strValue;
                OnPropertyChanged(nameof(Value));
                return true;
            }
            // otherwise try to have the decorated selector take care of it
            else if (_decorated.TrySetValue(value))
            {
                //if succesfull clear the ele selector _value so the decorated selector is used
                _value = null;
                OnPropertyChanged(nameof(Value));
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_value == null)
            {
                _decorated.TrySetValue(instance);
            }
            else if (_identifingMember == null)
            {
                _info.SetValue(instance, _value);
            }
            else
            {
                _info.SetValue(instance, _identifingMember.GetValue(_elementManager.Elements.FirstOrDefault(x => x.Name == _value).Value));
            }
        }
    }
}
