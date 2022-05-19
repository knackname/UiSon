// UiSon, by Cameron Gale 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Command;
using UiSon.Element;
using UiSon.Notify.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// An expandable collection of one kind of <see cref="IEditorModule"/>
    /// </summary>
    public class CollectionModule : GroupModule, ICollectionModule
    {
        public override object Value
        {
            get
            {
                // some implimentations of ICollection<> may have rules about what can be added and throw an error
                // in that case refresh the old value back into the child moduels
                try
                {
                    var value = Activator.CreateInstance(_collectionType);

                    foreach (var member in _members)
                    {
                        _collectionAdd.Invoke(value, new object[] { member.Value });
                    }

                    _isValueBad = false;
                    return value;
                }
                catch
                {
                    _isValueBad= true;
                    _notifier.Notify($"{nameof(UiSonCollectionAttribute)} {Name}: Invalid entries, reverting.", "Invalid Collection Entries");
                    return null;
                }
            }
            set
            {
                if (value is IEnumerable enumerable
                    && value.GetType()
                            .GetInterfaces()
                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                            .GetGenericArguments()
                            .FirstOrDefault() == _entryType)
                {
                    _members.Clear();

                    foreach (var entry in enumerable)
                    {
                        var newMember = _factory.MakeCollectionEntryVM(this, _entryType, _attribute, _uiAttribute);
                        newMember.Value = entry;
                        _members.Add(newMember);
                    }

                    _isValueBad = false;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Members));
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        private bool _isValueBad;

        public bool CanModifyCollection => _canMoodifyCollection;
        private readonly bool _canMoodifyCollection;

        public override ModuleState State => _isValueBad ? ModuleState.Error : base.State;

        private readonly ValueMemberInfo _info;
        private readonly EditorModuleFactory _factory;
        private readonly ICollection<IEditorModule> _members;
        private readonly Type _entryType;
        private readonly Type _collectionType;
        private readonly MethodInfo _collectionAdd;
        private readonly UiSonCollectionAttribute _attribute;
        private readonly UiSonUiAttribute _uiAttribute;
        private readonly ModuleTemplateSelector _templateSelector;
        private readonly INotifier _notifier;

        /// <summary>
        /// Constructor
        /// </summary>
        public CollectionModule(ValueMemberInfo info,
                                ModuleTemplateSelector templateSelector,
                                string name,
                                int displayPriority,
                                bool canModifyCollection,
                                DisplayMode displayMode,
                                Type collectionType,
                                Type entryType,
                                UiSonUiAttribute uiAttribute,
                                UiSonCollectionAttribute collectionAttribute,
                                EditorModuleFactory factory,
                                INotifier notifier,
                                NotifyingCollection<IEditorModule> members,
                                bool hideName = false)
            : base(members, name, displayPriority, displayMode, hideName)
        {
            _info = info;
            _collectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            _collectionAdd = _collectionType.GetInterfaces()
                                            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                                            .GetMethod("Add");

            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));

            _members = members ?? throw new ArgumentNullException(nameof(members));

            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));

            _uiAttribute = uiAttribute;
            _attribute = collectionAttribute;
            _canMoodifyCollection = canModifyCollection;
        }

        private void AddEntry()
        {
            var newEntry = _factory.MakeCollectionEntryVM(this, _entryType, _attribute, _uiAttribute);
            newEntry.PropertyChanged += OnMemberPropertyChanged;
            _members.Add(newEntry);
            OnPropertyChanged(nameof(Members));
        }

        public void RemoveEntry(IEditorModule entry)
        {
            entry.PropertyChanged -= OnMemberPropertyChanged;
            _members.Remove(entry);
            OnPropertyChanged(nameof(Members));
        }

        public override void Read(object instance) => Value = _info.GetValue(instance);

        public override void Write(object instance) => _info.SetValue(instance, Value);

        public override IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridTemplateColumn
            {
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                Header = Name
            };

            var dt = new DataTemplate();
            var valBind = new Binding(path)
            {
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, valBind);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, _templateSelector);

            dt.VisualTree = contentPresenter;
            valCol.CellTemplate = dt;

            return new List<DataGridColumn>() { valCol };
        }

        #region Commands

        public ICommand AddEntryCommand => new UiSonActionCommand((s) => AddEntry());

        #endregion
    }
}
