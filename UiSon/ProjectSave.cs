// UiSon, by Cameron Gale 2021

using System.Collections.Generic;

namespace UiSon
{
    public class ProjectSave
    {
        public string LogoPath { get; set; } = "/Views/Images/logo.png";

        public string Description { get; set; } = "UiSon is a tool designed for C# projects using json assets. Including the UiSon.Attributes " +
            "lib into your project lets you to use attributes on your json-serializable classes and influence how they appear in the editor. " +
            "'null' is a reserved keyword that will set the value to null rather than a string.";

        public List<string> Assemblies { get; set; } = new List<string>();
    }
}
