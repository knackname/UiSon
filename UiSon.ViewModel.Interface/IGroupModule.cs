// UiSon, by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// For identification
    /// </summary>
    public interface IGroupModule : IEditorModule
    {
        /// <summary>
        /// The Group's display mode.
        /// </summary>
        DisplayMode DisplayMode { get; }

        /// <summary>
        /// Modules the group displays
        /// </summary>
        IEnumerable<IEditorModule> Members { get; }
    }
}
