// UiSon, by Cameron Gale 2022

using System;
using System.Windows.Input;
using UiSon.View.Interface;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// an editor module with a value 
    /// </summary>
    public interface IValueEditorModule : IEditorModule
    {
        /// <summary>
        /// The module's value.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The type the value should be for setting
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// The module's view
        /// </summary>
        IUiValueView View { get; }

        bool HasError { get; }

        ICommand CopyCommand { get; }
        ICommand PasteCommand { get; }
        ICommand ShowErrorCommand { get; }
    }
}
