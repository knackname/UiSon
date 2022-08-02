// UiSon, by Cameron Gale 2021

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Element
{
    /// <summary>
    /// Project save file.
    /// </summary>
    [UiSonElement(".uis")]
    public class ProjectSave
    {
        /// <summary>
        /// Description used by UiSon for this project.
        /// </summary>
        [UiSonTextEditUi]
        public string Description { get; set; } = "UiSon is a tool designed for C# projects using json assets. Including the UiSon.Attributes " +
            "library in your project lets you to use attributes on your json-serializable classes and influence how they appear in the editor. " +
            "'null' is a reserved keyword that will set the value to null rather than a string.";

        /// <summary>
        /// If assamblies are allowed to be added or removed from the project.
        /// </summary>
        [UiSonCheckboxUi]
        public bool AllowAssemblyMod { get; set; } = true;

        /// <summary>
        /// The skin used by UiSon for this project.
        /// </summary>
        [UiSonTextEditUi]
        public string Skin { get; set; } = "Dark";

        /// <summary>
        /// Paths realative to the save file's directory to assemblies used by this project.
        /// </summary>
        [UiSonTextEditUi]
        public List<string> Assemblies { get; set; } = new List<string>();

        /// <summary>
        /// Paths realative to the save file's directory to assemblies used by this project.
        /// </summary>
        [UiSonEncapsulatingUi]
        public Dictionary<string, string> CustomSkins { get; set; } = new Dictionary<string, string>();
    }
}
