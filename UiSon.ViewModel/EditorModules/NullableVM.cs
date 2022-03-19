// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UiSon.Extension;
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

                    // only create the decorated the first time its opened to prevent infinite loops from a element having an instance of itself
                    if (!value && Decorated == null)
                    {
                        Decorated = MakeEditor?.Invoke();

                        // set to null so this can be cleaned up, it'll only be used once
                        MakeEditor = null;

                        Decorated?.SetValue(Activator.CreateInstance(_type));
                        OnPropertyChanged(nameof(Decorated));
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
        private MemberInfo _info;

        private Type _type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="makeEditor">function to make the decorated editor</param>
        /// <param name="info">info for member this module represents</param>
        public NullableVM(Func<IEditorModule> makeEditor, Type type, string name, int priority, MemberInfo info)
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
            var valCol = new DataGridCheckBoxColumn();
            valCol.Header = "null";

            var valBinding = new Binding(path + $".{nameof(IsNull)}");
            valBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            valCol.Binding = valBinding;

            var columns = new List<DataGridColumn>() { valCol };

            var decoratedColumns = Decorated?.GenerateColumns(path + $".{nameof(Decorated)}");

            if (decoratedColumns != null)
            {
                foreach (var column in decoratedColumns)
                {
                    columns.Add(column);
                }
            }

            return columns;
        }

        /// <summary>
        /// Reads from instance into this editor moduel
        /// </summary>
        /// <param name="instance">instance to read from</param>
        public void Read(object instance)
        {
            //set IsNull
            if (instance == null) { IsNull = true; }

            object read = null;

            if (_info is PropertyInfo prop)
            {
                read = prop.GetValue(instance);
            }
            else if (_info is FieldInfo field)
            {
                read = field.GetValue(instance);
            }
            else
            {
                throw new Exception("Attempting to read on an element without member info");
            }

            IsNull = read == null;

            // process element
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
            if (!IsNull)
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
                IsNull = false;
                return Decorated.SetValue(value);
            }
        }

        public object GetValueAs(Type type) => Decorated?.GetValueAs(type);

        public void UpdateRefs() => Decorated?.UpdateRefs();
    }
}
