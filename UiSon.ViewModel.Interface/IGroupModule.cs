// UiSon, by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.ViewModel.Interface
{
    /// <summary>
    /// A group of other editor modules.
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
        IList<IEditorModule> Members { get; }
    }
}
