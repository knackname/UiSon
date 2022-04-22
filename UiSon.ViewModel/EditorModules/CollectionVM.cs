// UiSon, by Cameron Gale 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// An expandable collection of one kind of <see cref="IEditorModule"/>
    /// </summary>
    public class CollectionVM : GroupVM, ICollectionVM
    {
        public Visibility ModifyVisibility { get; private set; }

        private EditorModuleFactory _factory;

        private ValueMemberInfo _info;

        private ObservableCollection<CollectionEntryVM> _members;

        private Type _entryType;

        private Type _collectionType;

        private MethodInfo _collectionAdd;

        private UiSonCollectionAttribute _enumerableAttribute;

        private UiSonUiAttribute _uiAttribute;

        private ElementTemplateSelector _templateSelector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="members"></param>
        /// <param name="entryType"></param>
        /// <param name="collectionType"></param>
        /// <param name="factory"></param>
        /// <param name="uiAttribute"></param>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="info"></param>
        /// <param name="modifiable"></param>
        /// <param name="displayMode"></param>
        /// <param name="attribute"></param>
        /// <param name="templateSelector"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CollectionVM(ObservableCollection<CollectionEntryVM> members,
                            Type entryType, Type collectionType, EditorModuleFactory factory,
                            UiSonUiAttribute uiAttribute,
                            string name, int priority, ValueMemberInfo info, 
                            bool modifiable,
                            DisplayMode displayMode,
                            UiSonCollectionAttribute attribute,
                            ElementTemplateSelector templateSelector)
            : base(members, name, priority, displayMode)
        {
            _collectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));
            _members = members ?? throw new ArgumentNullException(nameof(members));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));

            _collectionAdd = _collectionType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                .GetMethod("Add");

            _uiAttribute = uiAttribute;
            ModifyVisibility = modifiable ? Visibility.Visible : Visibility.Collapsed;
            _enumerableAttribute = attribute;
            _info = info;
        }

        public override void Read(object instance) => SetValue(_info.GetValue(instance));

        public override void Write(object instance) => _info.SetValue(instance, GetValue());

        public override bool SetValue(object value)
        {
            if (value != null && value.GetType().GetInterface(nameof(IEnumerable)) != null)
            {
                _members.Clear();

                foreach (var entry in (IEnumerable)value)
                {
                    var newEntry = _factory.MakeCollectionEntryVM(_members, _entryType, ModifyVisibility, _enumerableAttribute, _uiAttribute);
                    newEntry.SetValue(entry);
                    _members.Add(newEntry);
                }

                OnPropertyChanged(nameof(Members));

                return true;
            }

            return false;
        }

        public override object GetValueAs(Type type)
        {
            if (type == _collectionType)
            {
                return GetValue();
            }

            return null;
        }

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

        public ICommand AddEntry => new UiSonActionCommand((s) => _members.Add(_factory.MakeCollectionEntryVM(_members, _entryType, ModifyVisibility, _enumerableAttribute, _uiAttribute)));

        private object GetValue()
        {
            var value = Activator.CreateInstance(_collectionType);

            foreach (var member in _members)
            {
                _collectionAdd.Invoke(value, new[] { member.GetValueAs(_entryType) });
            }

            return value;
        }
    }
}
