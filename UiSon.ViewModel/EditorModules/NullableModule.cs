// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UiSon.Element;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A special kind of border including a "Null" checkbox to allow the user to set the value to null.
    /// Decorates other <see cref="IEditorModule"/>.
    /// </summary>
    public class NullableModule : NPCBase, IEditorModule, INullableModule
    {
        public object Value
        {
            get => IsNull ? null : Decorated.Value;
            set
            {
                if (value != Value)
                {
                    if (value == null)
                    {
                        IsNull = true;
                    }
                    else if (value.GetType() == _type)
                    {
                        IsNull = false;
                        Decorated.Value = value;
                    }

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public string Name => _name;
        private string _name;

        public int DisplayPriority => _displayPriority;
        private int _displayPriority;

        public bool IsNameVisible => !string.IsNullOrWhiteSpace(Name);

        public ModuleState State => IsNull ? ModuleState.Special : Decorated.State;

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
                        Decorated = MakeEditor();

                        // set to null so it can be freed, it's only needed the once
                        MakeEditor = null;

                        Decorated.PropertyChanged += OnDecoratedPropertyChanged;

                        Decorated.Value = Activator.CreateInstance(_type);
                    }

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsExpanded));
                    OnPropertyChanged(nameof(State));
                }
            }
        }
        private bool _isNull = true;

        /// <summary>
        /// Decorated module
        /// </summary>
        public IEditorModule Decorated { get; private set; }

        private Func<IEditorModule> MakeEditor;
        private readonly Type _type;
        private readonly ValueMemberInfo _info;
        private readonly ModuleTemplateSelector _templateSelector;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="makeEditor">function to make the decorated editor</param>
        /// <param name="info">info for member this module represents</param>
        public NullableModule(string name,
                              int displayPriority,
                              Type type,
                              ValueMemberInfo info,
                              ModuleTemplateSelector templateSelector,
                              Func<IEditorModule> makeEditor)
        {
            MakeEditor = makeEditor ?? throw new ArgumentNullException(nameof(makeEditor));
            _templateSelector = templateSelector ?? throw new ArgumentNullException(nameof(templateSelector));
            _type = type ?? throw new ArgumentNullException(nameof(type));

            _name = name;
            _info = info;
            _displayPriority = displayPriority;
        }

        private void OnDecoratedPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IEditorModule.Value):
                    OnPropertyChanged(nameof(Value));
                    IsNull = Decorated?.Value == null;
                    break;
            }
        }

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
            contentPresenter.SetValue(ContentPresenter.ContentTemplateSelectorProperty, _templateSelector);

            dt.VisualTree = contentPresenter;
            valCol.CellTemplate = dt;

            return new List<DataGridColumn>() { valCol };
        }

        public void Read(object instance)
        {
            if (_info.GetValue(instance) == null)
            {
                IsNull = true;
            }
            else
            {
                IsNull = false;
                Decorated.Read(instance);
            }
        }

        public void Write(object instance) => _info.SetValue(instance, IsNull ? null : Decorated.Value);
    }
}
