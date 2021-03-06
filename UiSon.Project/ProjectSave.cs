// UiSon, by Cameron Gale 2021

using System.Collections.Generic;
using UiSon.Attribute;
using UiSon.View.Interface;

namespace UiSon.Project
{
    /// <summary>
    /// Project save file, serialized into json
    /// </summary>
    [UiSonElement(".uis")]
    [UiSonArray("Skin_AsString", typeof(Skin), typeof(string))]
    public class ProjectSave
    {
        /// <summary>
        /// Path to the logo used by the editor, done this way so it can be customized by the user
        /// </summary>
        [UiSonTextEditUi]
        public string LogoPath { get; set; }

        /// <summary>
        /// Description used by the editor, done this way so it can be customized by the user
        /// </summary>
        [UiSonTextEditUi]
        public string Description { get; set; } = "UiSon is a tool designed for C# projects using json assets. Including the UiSon.Attributes " +
            "lib into your project lets you to use attributes on your json-serializable classes and influence how they appear in the editor. " +
            "'null' is a reserved keyword that will set the value to null rather than a string.";

        /// <summary>
        /// If assamblies are allowed to be added or removed from the project
        /// </summary>
        [UiSonCheckboxUi]
        public bool AllowAssemblyMod { get; set; } = true;

        [UiSonSelectorUi("Skin_As_String")]
        public Skin Skin { get; set; } = Skin.Light;

        /// <summary>
        /// Paths to assemblies used by this project
        /// </summary>
        [UiSonCollection]
        [UiSonTextEditUi]
        public List<string> Assemblies { get; set; } = new List<string>();
    }
}
