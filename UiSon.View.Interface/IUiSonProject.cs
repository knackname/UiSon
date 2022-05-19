// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Element;

namespace UiSon.View.Interface
{
    public interface IUiSonProject : INotifyPropertyChanged
    {
        /// <summary>
        /// The project's name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The project's skin
        /// </summary>
        public string? Skin { get; set; }

        /// <summary>
        /// The directory the project is saved to
        /// </summary>
        public string? SaveFileDirectory { get; set; }

        /// <summary>
        /// If the project has unsaved changes.
        /// </summary>
        bool HasUnsavedChanges { get; }

        /// <summary>
        /// Saves the project to its filepath.
        /// </summary>
        /// <returns>Returns true if successful. False otherwise.</returns>
        bool Save();

        string Description { get; }

        bool AllowAssemblyMod { get; }

        IEnumerable<IAssemblyView> Assemblies { get; }

        IEnumerable<IElementManager> ElementManagers { get; }

        void AddAssembly(string path);

        void RemoveAssembly(IAssemblyView assembly);

        IEnumerable<KeyValuePair<string, string[]>> StringArrays { get; }
    }
}
