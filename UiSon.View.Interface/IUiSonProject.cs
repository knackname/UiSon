// UiSon, by Cameron Gale 2022

using System.ComponentModel;

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

        /// <summary>
        /// 
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 
        /// </summary>
        bool AllowAssemblyMod { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IAssemblyView> Assemblies { get; }

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<IElementManager> ElementManagers { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        void AddAssembly(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        void RemoveAssembly(IAssemblyView assembly);

        /// <summary>
        /// 
        /// </summary>
        IEnumerable<KeyValuePair<string, string[]>> StringArrays { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementManager"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void ImportElement(IElementManager elementManager, string name, object value);
    }
}
