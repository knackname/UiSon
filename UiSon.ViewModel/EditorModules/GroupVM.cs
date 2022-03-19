// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UiSon.Attribute;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A group of other editor modules, not deigned to be expandable
    /// </summary>
    public class GroupVM : NPCBase, IEditorModule
    {
        public string Name { get; private set; }

        public int Priority { get; private set; }

        public IEnumerable<IEditorModule> Members { get; protected set; }

        public DisplayMode DisplayMode { get; private set; }

        public Visibility IsNameVisible => string.IsNullOrWhiteSpace(Name) ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Constructor
        /// </summary>
        public GroupVM(IEnumerable<IEditorModule> members, string name = null, int priority = 0,
                       DisplayMode displayMode = DisplayMode.Vertial)
        {
            Name = name;
            Priority = priority;
            Members = members ?? throw new ArgumentNullException(nameof(members));
            DisplayMode = displayMode;
        }

        /// <summary>
        /// Generates data grid column(s) for parent grids
        /// </summary>
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

        /// <summary>
        /// Writes this editor's element's value to the instance
        /// </summary>
        public virtual void Read(object instance)
        {
            if (instance == null) { return; }

            foreach (var member in Members)
            {
                member.Read(instance);
            }
        }

        /// <summary>
        /// Reads data from instance and set's this editor's element's value to it
        /// </summary>
        public virtual void Write(object instance)
        {
            foreach (var member in Members)
            {
                member.Write(instance);
            }
        }

        public void UpdateRefs()
        {
            foreach (var member in Members)
            {
                member.UpdateRefs();
            }
        }

        public virtual bool SetValue(object value) => throw new NotImplementedException();

        public virtual object GetValueAs(Type type) => throw new NotImplementedException();
    }
}
