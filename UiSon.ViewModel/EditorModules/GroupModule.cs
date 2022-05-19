// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A group of other editor modules, not deigned to be expandable
    /// </summary>
    public class GroupModule : NPCBase, IEditorModule, IGroupModule
    {
        public virtual object Value
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public IEnumerable<IEditorModule> Members => _members.OrderByDescending(x => x.DisplayPriority);
        private readonly IEnumerable<IEditorModule> _members;

        public string Name => _name;
        private readonly string _name;

        public int DisplayPriority => _displayPriority;
        private readonly int _displayPriority;

        public DisplayMode DisplayMode => _displayMode;
        private readonly DisplayMode _displayMode;

        public bool IsNameVisible => !_hideName && !string.IsNullOrWhiteSpace(Name);

        public virtual ModuleState State => _members.Any(x => x.State == ModuleState.Error) ? ModuleState.Error : ModuleState.Normal;

        private readonly bool _hideName;

        /// <summary>
        /// Constructor
        /// </summary>
        public GroupModule(IEnumerable<IEditorModule> members,
                              string name,
                              int displayPriority,
                              DisplayMode displayMode,
                              bool hideName = false)
        {
            _members = members ?? throw new ArgumentNullException(nameof(members));

            foreach (var member in members)
            {
                member.PropertyChanged += OnMemberPropertyChanged;
            }

            _hideName = hideName;
            _name = name;
            _displayPriority = displayPriority;
            _displayMode = displayMode;
        }

        protected void OnMemberPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IEditorModule.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IEditorModule.State):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        public virtual void Read(object instance)
        {
            foreach (var member in Members)
            {
                member.Read(instance);
            }
        }

        public virtual void Write(object instance)
        {
            foreach (var member in Members)
            {
                member.Write(instance);
            }
        }

        public virtual IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var columns = new List<DataGridColumn>();

            var i = 0;
            foreach (var member in Members)
            {
                foreach (var col in member.GenerateColumns(path + $".{nameof(Members)}[{i}]"))
                {
                    columns.Add(col);
                }
                i++;
            }

            return columns;
        }
    }
}
