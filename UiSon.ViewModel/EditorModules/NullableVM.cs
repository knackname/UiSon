// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UiSon.Element;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A special kind of border including a "Null" checkbox to allow the user to set the value to null.
    /// Decorates other <see cref="IEditorModule"/>.
    /// </summary>
    public class NullableVM : NPCBase, IEditorModule
    {
        /// <summary>
        /// The module's name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// if the expanded is expanded
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded && !IsNull;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _isExpanded = false;

        /// <summary>
        /// state of the "null" chackbox
        /// </summary>
        public bool IsNull
        {
            get => _isNull;
            set
            {
                if (_isNull != value)
                {
                    _isNull = value;

                    // if decorated is null, init it
                    if (!value && Decorated == null)
                    {
                        SetValue(Activator.CreateInstance(_type));
                    }

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsExpanded));
                    OnPropertyChanged(nameof(TextColor));
                }
            }
        }
        private bool _isNull = true;

        /// <summary>
        /// Color of text display
        /// </summary>
        public Brush TextColor => IsNull ? UiSonColors.Red : UiSonColors.Black;

        /// <summary>
        /// Decorated module
        /// </summary>
        public IEditorModule Decorated { get; private set; }

        /// <summary>
        /// Display priority
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// Wether or not the editor's name is visible
        /// </summary>
        public Visibility IsNameVisible => Decorated?.IsNameVisible ?? Visibility.Collapsed;

        private Func<IEditorModule> MakeEditor;

        /// <summary>
        /// info for member this module represents
        /// </summary>
        private ValueMemberInfo _info;

        private Type _type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="makeEditor">function to make the decorated editor</param>
        /// <param name="info">info for member this module represents</param>
        public NullableVM(Func<IEditorModule> makeEditor, Type type, string name, int priority, ValueMemberInfo info)
        {
            MakeEditor = makeEditor ?? throw new ArgumentNullException(nameof(makeEditor));
            _type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name;
            Priority = priority;
            _info = info;
        }

        /// <summary>
        /// Generates columns to represent this editor moduel in a grid
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var valCol = new DataGridTemplateColumn();
            valCol.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            valCol.Header = Name;

            var dt = new DataTemplate();
            var valBind = new Binding(path);
            valBind.Mode = BindingMode.OneWay;
            valBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, valBind);
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, new ElementTemplateSelector());// don't make a new one, pass it in TODO

            dt.VisualTree = contentPresenter;
            valCol.CellTemplate = dt;

            return new List<DataGridColumn>() { valCol };
        }

        /// <summary>
        /// Reads from instance into this editor moduel
        /// </summary>
        /// <param name="instance">instance to read from</param>
        public void Read(object instance)
        {
            IsNull = _info.GetValue(instance) == null;

            if (!IsNull)
            {
                Decorated.Read(instance);
            }
        }

        /// <summary>
        /// Writes from this editor moduel to the instance
        /// </summary>
        /// <param name="instance">instance to write to</param>
        public void Write(object instance)
        {
            if (IsNull)
            {
                _info.SetValue(instance, null);
            }
            else
            {
                Decorated.Write(instance);
            }
        }

        public bool SetValue(object value)
        {
            if (value == null)
            {
                IsNull = true;
                return true;
            }
            else
            {
                //generate the decorated if nessisary
                if (Decorated == null)
                {
                    Decorated = MakeEditor.Invoke();

                    // set to null so this can be cleaned up, it'll only be used this once
                    MakeEditor = null;

                    Decorated.PropertyChanged += (s,e) => OnPropertyChanged(nameof(Decorated));

                    OnPropertyChanged(nameof(Decorated));
                }

                //attempt to set value
                if (Decorated.SetValue(value))
                {
                    IsNull = false;
                    return true;
                }
            }

            return false;
        }

        public object GetValueAs(Type type) => Decorated?.GetValueAs(type);

        public void UpdateRefs() => Decorated?.UpdateRefs();
    }
}
