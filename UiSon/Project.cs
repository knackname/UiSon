// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Controls;
using UiSon.Notify.Interface;
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
        /// If assamblies are allowed to be added or removed from the project
        /// </summary>
        public bool AllowAssemblyMod => _projectSave.AllowAssemblyMod;

        /// <summary>
        /// Assemblies uesd by this project
        /// </summary>
        public IEnumerable<AssemblyVM> Assemblies => _assemblies;
        private ObservableCollection<AssemblyVM> _assemblies = new ObservableCollection<AssemblyVM>();

        /// <summary>
        /// Element managers used by this project
        /// </summary>
        public IEnumerable<ElementManager> ElementManagers => _elementManagers;
        private Collection<ElementManager> _elementManagers;

        /// <summary>
        /// if project has unsaved changes
        /// </summary>
        public bool UnsavedChanges
        {
            get => _unsavedChanges;
            set
            {
                if (_unsavedChanges != value)
                {
                    _unsavedChanges = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _unsavedChanges = false;

        /// <summary>
        /// The project's name
        /// </summary>
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
        private TabControl _tabController;

        /// <summary>
        /// editor module factory
        /// </summary>
        private EditorModuleFactory _editorModuleFactory;

        private INotifier _notifier;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="projectSave">project save file</param>
        /// <param name="tabController">tab controller</param>
        /// <param name="editorModuleFactory">factory for making editor modules</param>
        /// <param name="elementManagers">collection of element managers</param>
        /// <param name="notifier">The notifier</param>
        public Project(ProjectSave projectSave, TabControl tabController, EditorModuleFactory editorModuleFactory, Collection<ElementManager> elementManagers, INotifier notifier)
        {
            _projectSave = projectSave ?? throw new ArgumentNullException(nameof(projectSave));
            _tabController = tabController ?? throw new ArgumentNullException(nameof(tabController));
            _elementManagers = elementManagers ?? throw new ArgumentNullException(nameof(elementManagers));
            _editorModuleFactory = editorModuleFactory ?? throw new ArgumentNullException(nameof(editorModuleFactory));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        /// <summary>
        /// Adds the assembly from the given path to the project
        /// </summary>
        /// <param name="path">File path to the assembly</param>
        public void AddAssembly(string path)
        {
            // if assembly is already loaded just refresh it
            var assembly = _assemblies.FirstOrDefault(x => x.Path == path);
            if (assembly != null)
            {
                assembly.RefreshElements();
            }

            // add new assembly VM
            _assemblies.Add(new AssemblyVM(path, this, _elementManagers, _tabController, _editorModuleFactory, _notifier));
        }

        /// <summary>
        /// Removes an assembly from the project
        /// </summary>
        /// <param name="assembly"></param>
        public void RemoveAssembly(AssemblyVM assembly) => _assemblies.Remove(assembly);

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

            if (_projectSave == null)
            {
                throw new Exception($"Failed to deserialize project {path}");
            }

            foreach (var assembly in _projectSave.Assemblies)
            {
                AddAssembly(assembly);
            }

            var loads = new List<LoadStruct>();

            foreach (var elementManager in ElementManagers)
            {
                // all managers need to load their elements, then load their information because elements reference one another
                var elementDir = Path.Combine(Path.GetDirectoryName(path), elementManager.ElementName);

                if (Directory.Exists(elementDir))
                {
                    _notifier.StartCashe();

                    foreach (var file in Directory.GetFiles(elementDir))
                    {
                        var name = Path.GetFileName(file);

                        if (name.EndsWith(elementManager.Extension))
                        {
                            // make sure file is valid first
                            var instance = JsonSerializer.Deserialize(File.ReadAllText(file), elementManager.ManagedType, new JsonSerializerOptions() { IncludeFields = elementManager.IncludesFields });

                            // ignore bad files
                            if (instance == null)
                            {
                                _notifier.Notify("Deserialize failure", $"Ignoring {name}");
                            }
                            else
                            {
                                name = name.Substring(0, name.Length - elementManager.Extension.Length);

                                elementManager.NewElement(name);

                                loads.Add(new LoadStruct() { ElementManager = elementManager, Name = name, Instance = instance});
                            }
                        }
                    }

                    _notifier.EndCashe();
                }
            }

            // now that everything is created, load all the data. Some data includes the names of other elements so it's nessisary to load after they all exist.
            foreach (var entry in loads)
            {
                entry.ElementManager.Load(entry.Name, entry.Instance);
            }

            // and now update refs
            foreach (var manager in ElementManagers)
            {
                manager.UpdateRefs();
            }
        }
        
        /// <summary>
        /// Helper struct for the load process
        /// </summary>
        private struct LoadStruct
        {
            public ElementManager ElementManager;
            public string Name;
            public object Instance;
        }
    }
}
