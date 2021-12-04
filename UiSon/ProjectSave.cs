// UiSon, by Cameron Gale 2021

using System.Collections.Generic;

namespace UiSon
{
    /// <summary>
    /// Project save file, serialized into json
    /// </summary>
    public class ProjectSave
    {
        /// <summary>
        /// Path to the logo used by the editor, done this way so it can be customized by the user
        /// </summary>
        public string LogoPath { get; set; } = "/Views/Images/logo.png";

        /// <summary>
        /// Description used by the editor, done this way so it can be customized by the user
        /// </summary>
        public string Description { get; set; } = "UiSon is a tool designed for C# projects using json assets. Including the UiSon.Attributes " +
            "lib into your project lets you to use attributes on your json-serializable classes and influence how they appear in the editor. " +
            "'null' is a reserved keyword that will set the value to null rather than a string.";

        /// <summary>
        /// Paths to assemblies used by this project
        /// </summary>
        public List<string> Assemblies { get; set; } = new List<string>();
    }
}
