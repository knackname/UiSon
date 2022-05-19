// UiSon, by Cameron Gale 2022

namespace UiSon.Project.Interface
{
    public interface IUiSonProject
    {
        /// <summary>
        /// If the project has unsaved changes.
        /// </summary>
        bool HasUnsavedChanges { get; }

        /// <summary>
        /// The save file's path.
        /// </summary>
        string FilePath { set; }

        /// <summary>
        /// The skin used by the project.
        /// </summary>
        Skin Skin { get; set; }

        /// <summary>
        /// Saves the project to its filepath.
        /// </summary>
        /// <returns>Returns true if successful. False otherwise.</returns>
        bool Save();
    }
}
