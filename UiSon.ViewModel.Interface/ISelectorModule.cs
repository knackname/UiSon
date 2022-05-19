// UiSon, by Cameron Gale 2022

using System.Collections.Generic;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// For identification
    /// </summary>
    public interface ISelectorModule : IEditorModule
    {
        /// <summary>
        /// Options to select from
        /// </summary>
        IEnumerable<string> Options { get; }
    }
}
