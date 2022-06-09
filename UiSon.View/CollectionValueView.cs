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
    public class CollectionValueView : EncapsulatingView, ICollectionValueView
    {
        /// <inheritdoc/>
        public override object? Value
        {
            get
            {
                var value = base.Value;

                // some implimentations of ICollection<> may have rules about what can be added beyond type and throw an error
                try
                {
                    foreach (var entry in _entries)
                    {
                        if (entry.Value.TryCast(_entryType, out object cast))
                        {
                            _collectionAdd.Invoke(value, new object[] { cast });
                        }
                    }

                    SetIsValueBad(false);
                    return value;
                }
                catch
                {
                    SetIsValueBad(true);
                    return value;
                }
            }
        }

        /// <summary>
        /// The Collection Attribute
        /// </summary>
        public UiSonCollectionAttribute? CollectionAttribute { get; private set; }

        /// <summary>
        /// If the collection's members don't make a valid collection of this view's type.
        /// </summary>
        public override bool IsValueBad => _isValueBad || base.IsValueBad;
        protected void SetIsValueBad(bool value)
        {
            if (_isValueBad != value)
            {
                _isValueBad = value;
                OnPropertyChanged(nameof(IsValueBad));
            }
        }
        private bool _isValueBad;

        /// <inheritdoc/>
        public IEnumerable<IUiValueView> Entries => _entries;

        /// <inheritdoc/>
        public bool IsModifiable {get; private set;}

        protected readonly List<IUiValueView> _entries = new List<IUiValueView>();

        private readonly ViewFactory _factory;
        protected readonly Type _entryType;
        protected readonly MethodInfo _collectionAdd;
        private readonly UiSonUiAttribute? _entryAttribute;
        private readonly bool _autoGenerateMemberAttributes;

        public CollectionValueView(ViewFactory factory,
                                   Type type,
                                   bool autoGenerateMemberAttributes,
                                   Type entryType,
                                   UiSonUiAttribute entryAttribute,
                                   bool canModify,
                                   int displayPriority,
                                   string? name,
                                   DisplayMode displayMode,
                                   ValueMemberInfo? info,
                                   IReadWriteView[] members)
            :base (type, displayPriority, name, displayMode, info, members)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            var collectionAdd = type.GetInterfaces()
                                  .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                                  .GetMethod("Add");

            _collectionAdd = collectionAdd ?? throw new ArgumentException(nameof(type));

            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));

            _entryAttribute = entryAttribute ?? throw new ArgumentNullException(nameof(entryAttribute));

            _autoGenerateMemberAttributes = autoGenerateMemberAttributes;

            IsModifiable = canModify;
        }

        private void OnEntryPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.Value):
                    // get Value to refresh IsValueBad
                    var _ = Value;
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        /// <inheritdoc/>
        public IUiValueView AddEntry()
        {
            var newEntry = _factory.MakeView(_entryType,
                                             _autoGenerateMemberAttributes,
                                             null,
                                             _entryAttribute,
                                             CollectionAttribute);
            // non-value types can init as null, their null buffers will init them
            if (_entryType.IsValueType)
            {
                newEntry.TrySetValue(_entryType.GetDefaultValue());
            }

            newEntry.PropertyChanged += OnEntryPropertyChanged;

            _entries.Add(newEntry);

            // get value to update IsValueBad
            var _ = Value;

            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(DisplayValue));

            return newEntry;
        }

        /// <inheritdoc/>
        public void RemoveEntry(IUiValueView entry)
        {
            entry.PropertyChanged -= OnEntryPropertyChanged;
            _entries.Remove(entry);

            // get value to update IsValueBad
            var _ = Value;

            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(DisplayValue));
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            if (base.TrySetValue(value))
            {
                if (value is IEnumerable enumerable
                    && value.GetType()
                            .GetInterfaces()
                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))?
                            .GetGenericArguments()
                            .FirstOrDefault() == _entryType)
                {
                    _entries.Clear();

                    foreach (var entry in enumerable)
                    {
                        var newMember = _factory.MakeView(_entryType, _autoGenerateMemberAttributes, null, _entryAttribute, CollectionAttribute);
                        newMember.TrySetValueFromRead(entry);
                        _entries.Add(newMember);
                    }
                }

                SetIsValueBad(false);
                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(Members));
                OnPropertyChanged(nameof(Entries));
                return true;
            }    

            return false;
        }
    }
}
