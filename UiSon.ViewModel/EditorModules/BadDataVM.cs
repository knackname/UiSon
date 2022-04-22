// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Dummy moduel, returned in the case of a failed ui module creation
    /// </summary>
    public class BadDataVM : NPCBase, IEditorModule
    {
        public string Name { get; private set; }

        public int Priority => 0;

        public Visibility IsNameVisible => Visibility.Visible;

        /// <summary>
        /// constructor
        /// </summary>
        public BadDataVM(string error)
        {
            Name = error;
        }

        public IEnumerable<DataGridColumn> GenerateColumns(string path) => new List<DataGridColumn>();

        public void Read(object instance)
        {
            //not implimented
        }

        public void Write(object instance)
        {
            //not implimented
        }

        public bool SetValue(object value) => false;

        public object GetValueAs(Type type) => null;

        public void UpdateRefs()
        {
            //not implimented
        }
    }
}
