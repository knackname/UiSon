// UiSon, by Cameron Gale 2022

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Command;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// An expandable collection of one kind of <see cref="IEditorModule"/>
    /// </summary>
    public class CollectionVM : GroupVM, ICollectionVM
    {
        public CollectionStyle Style { get; private set; }

        public Visibility ModifyVisibility { get; private set; }

        private EditorModuleFactory _factory;

        private MemberInfo _info;

        private ObservableCollection<CollectionEntryVM> _members;

        private Type _entryType;

        private Type _collectionType;

        private MethodInfo _collectionAdd;

        private UiSonCollectionAttribute _enumerableAttribute;

        private IUiSonUiAttribute _uiAttribute;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="members"></param>
        /// <param name="entryType"></param>
        /// <param name="factory"></param>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        /// <param name="info"></param>
        /// <param name="modifiable"></param>
        /// <param name="displayMode"></param>
        /// <param name="collectionStyle"></param>
        /// <param name="attribute"></param>
        public CollectionVM(ObservableCollection<CollectionEntryVM> members,
                            Type entryType, Type collectionType, EditorModuleFactory factory,
                            IUiSonUiAttribute uiAttribute,
                            string name, int priority, MemberInfo info, 
                            bool modifiable = true,
                            DisplayMode displayMode = DisplayMode.Vertial,
                            CollectionStyle collectionStyle = CollectionStyle.Stack,
                            UiSonCollectionAttribute attribute = null)
            : base(members, name, priority, displayMode)
        {
            _collectionType = collectionType ?? throw new ArgumentNullException(nameof(collectionType));
            _entryType = entryType ?? throw new ArgumentNullException(nameof(entryType));

            _collectionAdd = _collectionType.GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
                .GetMethod("Add");

            _members = members ?? throw new ArgumentNullException(nameof(members));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            _uiAttribute = uiAttribute;
            ModifyVisibility = modifiable ? Visibility.Visible : Visibility.Collapsed;
            _enumerableAttribute = attribute;
            _info = info;
            Style = collectionStyle;
        }

        public override void Read(object instance)
        {
            if (instance == null) { return; }

            // read info
            IEnumerable collection = null;

            if (_info is PropertyInfo prop)
            {
                collection = prop.GetValue(instance) as IEnumerable;
            }
            else if (_info is FieldInfo field)
            {
                collection = field.GetValue(instance) as IEnumerable;
            }
            else
            {
                throw new Exception("Attempting to read on an editor module without member info");
            }

            // insert
            _members.Clear();

            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var newEntry = MakeNewEntry();

                    if (newEntry != null)
                    {
                        newEntry.SetValue(item);
                        _members.Add(newEntry);
                    }
                }
            }

            OnPropertyChanged(nameof(Members));
        }

        public override void Write(object instance)
        {
            var value = Activator.CreateInstance(_collectionType);

            foreach (var member in _members)
            {
                _collectionAdd.Invoke(value, new[] { member.GetValueAs(_entryType) });
            }

            if (_info is PropertyInfo prop)
            {
                prop.SetValue(instance, value);
            }
            else if (_info is FieldInfo field)
            {
                field.SetValue(instance, value);
            }
            else
            {
                throw new Exception("Attempting to write on an element without member info");
            }
        }

        public override bool SetValue(object value)
        {
            if (value != null && value.GetType().GetInterface(nameof(IEnumerable)) != null)
            {
                _members.Clear();

                foreach (var entry in (IEnumerable)value)
                {
                    var newEntry = MakeNewEntry();

                    if (newEntry != null)
                    {
                        newEntry.SetValue(entry);
                        _members.Add(newEntry);
                    }
                }

                OnPropertyChanged(nameof(Members));

                return true;
            }

            return false;
        }

        public override IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var columns = new List<DataGridColumn>();

            var valCol = new DataGridTemplateColumn();
            valCol.Header = Name;

            var dt = new DataTemplate();
            var valBind = new Binding(path);
            valBind.Mode = BindingMode.OneWay;
            valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, valBind);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, new ElementTemplateSelector());

            dt.VisualTree = contentPresenter;
            valCol.CellTemplate = dt;

            columns.Add(valCol);

            return columns;
        }

        private CollectionEntryVM MakeNewEntry(object source = null)
        {
            var newEntry = _factory.MakeCollectionEntryVM(_members, _entryType, ModifyVisibility, _enumerableAttribute, _uiAttribute);

            if (newEntry == null) { return null; }

            if (source is Button button
                && button.Parent is StackPanel mainPanel
                && mainPanel.Children[1] is DataGrid dataGrid)
            {
                // When a nullable becomes not null for the first time, it changes its Decorated, so columns need to be regenrated
                newEntry.PropertyChanged += (e, a) =>
                {
                    if (!_gridColumnsUpdated
                        && e is CollectionEntryVM collectionEntry
                        && a.PropertyName == "Decorated")
                    {
                        _gridColumnsUpdated = true;

                        // make a new sub data grid column thing woot
                        //var valCol = new DataGridTemplateColumn();
                        //valCol.Header = Name;

                        //var dt = new DataTemplate();
                        //var valBind = new Binding("Decorated.Decorated");
                        //valBind.Mode = BindingMode.OneWay;
                        //valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                        //var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
                        //contentPresenter.SetBinding(ContentPresenter.ContentProperty, valBind);
                        //contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
                        //contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, new ElementTemplateSelector());

                        //dt.VisualTree = contentPresenter;
                        //valCol.CellTemplate = dt;

                        //dataGrid.Columns.Add(valCol);

                        foreach (var column in collectionEntry.GenerateColumns("").Skip(1))
                        {
                            dataGrid.Columns.Add(column);
                        }
                    }
                };
            }

            return newEntry;
        }

        private bool _gridColumnsUpdated = false;

        public ICommand AddEntry => new UiSonActionCommand((s) =>
        {
            var newEntry = MakeNewEntry(s);
            if (newEntry != null)
            {
                _members.Add(newEntry);
            }
        });
    }
}
