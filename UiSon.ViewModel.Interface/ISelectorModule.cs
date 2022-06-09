// UiSon, by Cameron Gale 2022

using System.Collections.Generic;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A module with options to choose from.
    /// </summary>
    public interface ISelectorModule : IEditorModule
    {
        /// <summary>
        /// Options to select from
        /// </summary>
        IEnumerable<string> Options { get; }
    }
}
