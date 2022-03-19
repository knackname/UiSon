// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Describes a module for the element editor
    /// </summary>
    public interface IEditorModule : INotifyPropertyChanged
    {
        /// <summary>
        /// Display name for the editor
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The editors display priority
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Whether or not the editor's name is visible
        /// </summary>
        public Visibility IsNameVisible { get; }

        /// <summary>
        /// Generates data grid column(s) for parent grids
        /// </summary>
        public IEnumerable<DataGridColumn> GenerateColumns(string path);

        /// <summary>
        /// Writes this editor's element's value to the instance
        /// </summary>
        public void Read(object instance);

        /// <summary>
        /// Reads data from instance and set's this editor's element's value to it
        /// </summary>
        public void Write(object instance);

        /// <summary>
        /// Attempts to set the value to the input.
        /// </summary>
        /// <param name="value">input value</param>
        /// <returns>True if set successfully, false otherwise</returns>
        bool SetValue(object value);

        /// <summary>
        /// Returns the element's value as the given type
        /// </summary>
        /// <param name="type">The type to retreieve the value as</param>
        /// <returns>The element's value as the type</returns>
        object GetValueAs(Type type);

        void UpdateRefs();
    }
}
