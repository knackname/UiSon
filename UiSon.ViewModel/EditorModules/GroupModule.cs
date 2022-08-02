// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A group of other editor modules, not expandable
    /// </summary>
    public class GroupModule : NPCBase, IGroupModule
    {
        /// <inheritdoc/>
        public string Name => _name;
        private readonly string _name;

        /// <inheritdoc/>
        public int DisplayPriority => _displayPriority;
        private readonly int _displayPriority;

        /// <inheritdoc/>
        public DisplayMode DisplayMode => _displayMode;
        private readonly DisplayMode _displayMode;

        /// <inheritdoc/>
        public virtual ModuleState State => ModuleState.Normal;

        /// <inheritdoc/>
        public string StateJustification => null;

        /// <inheritdoc/>
        public IList<IEditorModule> Members => _members;
        private readonly IEditorModule[] _members;

        public GroupModule(string name,
                           int displayPriority,
                           DisplayMode displayMode,
                           IEditorModule[] members)
        {
            _members = members ?? throw new ArgumentNullException(nameof(members));

            foreach (var member in members)
            {
                member.PropertyChanged += OnMemberPropertyChanged;
            }

            _name = name;
            _displayPriority = displayPriority;
            _displayMode = displayMode;
        }

        private void OnMemberPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IEditorModule.State):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        /// <inheritdoc/>
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
