// UiSon, by Cameron Gale 2022

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using UiSon.Element;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// Describes a representing a value that can be get/set/read/write -ed
    /// </summary>
    public interface IEditorModule : INotifyPropertyChanged
    {
        /// <summary>
        /// The module's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Disctates the order in which modules are displayed
        /// </summary>
        int DisplayPriority { get; }

        /// <summary>
        /// The module's current state.
        /// </summary>
        ModuleState State { get; }

        /// <summary>
        /// The reason for the current state.
        /// </summary>
        string StateJustification { get; }

        /// <summary>
        /// Generates data grid columns for parent data grids.
        /// </summary>
        /// <param name="bindingPath">The binding path.</param>
        /// <returns>An enumerable of the generated columns.</returns>
        IEnumerable<DataGridColumn> GenerateColumns(string bindingPath);
    }
}
