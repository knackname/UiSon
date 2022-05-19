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
    public interface IEditorModule : INotifyPropertyChanged, INamedOrderedViewModel
    {
        /// <summary>
        /// The moduel's value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The module's current state
        /// </summary>
        ModuleState State { get; }

        /// <summary>
        /// 
        /// </summary>
        void Read(object instance);

        /// <summary>
        /// 
        /// </summary>
        void Write(object instance);

        /// <summary>
        /// Generates data grid columns for parent data grids.
        /// </summary>
        /// <param name="bindingPath">The binding path.</param>
        /// <returns>An enumerable of the generated columns.</returns>
        public IEnumerable<DataGridColumn> GenerateColumns(string bindingPath);
    }
}
