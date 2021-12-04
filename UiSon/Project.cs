// UiSon, by Cameron Gale 2021

using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text.Json;
using System;
using System.Windows.Controls;
using UiSon.ViewModel;

namespace UiSon
{
    /// <summary>
    /// Project class, constains editor data for elements and assemblies
    /// </summary>
    public class Project : NPCBase
    {
        /// <summary>
        /// Path to the logo
        /// </summary>
        public string LogoPath => _projectSave.LogoPath;

        /// <summary>
        /// Discription displayed in editor
        /// </summary>
        public string Description => _projectSave.Description;

        /// <summary>
        /// Assemblies uesd by this project
        /// </summary>
        public IEnumerable<AssemblyVM> Assemblies => _assemblies;
        private ObservableCollection<AssemblyVM> _assemblies = new ObservableCollection<AssemblyVM>();

        /// <summary>
        /// Element managers used by this project
        /// </summary>
        public IEnumerable<ElementManager> ElementManagers => _elementManagers;
        private ObservableCollection<ElementManager> _elementManagers = new ObservableCollection<ElementManager>();

        /// <summary>
        /// Wether or not this project has unsaved changes
        /// </summary>
        public bool UnsavedChanges { get; private set; } = false;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _name = "New Project";

        /// <summary>
        /// Save file for the project
        /// </summary>
        private ProjectSave _projectSave;

        /// <summary>
        /// Tab controller from the ui
        /// </summary>
        private TabControl _controller;

        /// <summary>
        /// Eleemtn factory
        /// </summary>
        private ElementFactory _elementFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="projectSave">project save file</param>
        /// <param name="controller">tab controller</param>
        public Project(ProjectSave projectSave, TabControl controller)
        {
            _projectSave = projectSave ?? throw new ArgumentNullException(nameof(projectSave));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _elementFactory = new ElementFactory(_elementManagers);//move out
        }

        /// <summary>
        /// Adds the assembly from the given path to the project
        /// </summary>
        /// <param name="path">File path to the assembly</param>
        public void AddAssembly(string path)
        {
            var dll = Assembly.LoadFile(path);
            if (dll == null)
            {
                // inform error
            }
            else
            {
                var assembly = _assemblies.FirstOrDefault(x => x.Path == path);
                if (assembly == null)
                {
                    assembly = new AssemblyVM(path, this, _elementManagers, _controller, _elementFactory);

                    _assemblies.Add(assembly);
                }
                else
                {
                    assembly.RefreshElements();
                }
            }
        }

        /// <summary>
        /// Removes an assembly from the project
        /// </summary>
        /// <param name="assembly"></param>
        public void RemoveAssembly(AssemblyVM assembly)
        {
            _assemblies.Remove(assembly);
        }

        /// <summary>
        /// Saves the project to the given path
        /// </summary>
        /// <param name="path">path to save the project to</param>
        public void Save(string path)
        {
            Name = Path.GetFileName(path);
            _projectSave.Assemblies = _assemblies.Select(x => x.Path).ToList();

            File.WriteAllText(path, JsonSerializer.Serialize(_projectSave));

            foreach (var elementManager in ElementManagers)
            {
                elementManager.Save(Path.GetDirectoryName(path));
            }
        }

        /// <summary>
        /// Loads the project from the given path
        /// </summary>
        /// <param name="path">path to save file</param>
        public void Load(string path)
        {
            Name = Path.GetFileName(path);

            _projectSave = JsonSerializer.Deserialize<ProjectSave>(File.ReadAllText(path));

            foreach (var assemby in _projectSave.Assemblies)
            {
                AddAssembly(assemby);
            }

            foreach (var elementManager in ElementManagers)
            {
                //all managers need to load their elements, then load their information because elements reference one another
                var elementDir = Path.Combine(Path.GetDirectoryName(path), elementManager.ElementName);

                if (Directory.Exists(elementDir))
                {
                    foreach (var file in Directory.GetFiles(elementDir))
                    {
                        var name = Path.GetFileName(file);

                        if (name.EndsWith(elementManager.Extension))
                        {
                            name = name.Substring(0, name.Length - elementManager.Extension.Length);

                            elementManager.NewElement(name);

                            elementManager.Load(name, File.ReadAllText(file));
                        }
                    }
                }
            }
        }
    }
}
