// UiSon, by Cameron Gale 2021

using System.Reflection;
using System.Text.Json;
using UiSon.Element;
using UiSon.Notify.Interface;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// Project class, constains editor data for elements and assemblies
    /// </summary>
    public class UiSonProjectView : NPCBase, IUiSonProject
    {
        public string? Skin
        {
            get => _projectSave.Skin;
            set => _projectSave.Skin = value; 
        }

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
        public IEnumerable<IAssemblyView> Assemblies => _assemblies.OrderBy(x => x.Path);
        private List<IAssemblyView> _assemblies = new List<IAssemblyView>();

        /// <summary>
        /// Element managers used by this project
        /// </summary>
        public IEnumerable<IElementManager> ElementManagers => _assemblies.SelectMany(x => x.ElementManagers);

        public IEnumerable<KeyValuePair<string, string[]>> StringArrays => _assemblies.SelectMany(x => x.StringArrays);

        /// <summary>
        /// if project has unsaved changes
        /// </summary>
        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            set
            {
                if (_hasUnsavedChanges != value)
                {
                    _hasUnsavedChanges = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _hasUnsavedChanges = false;

        /// <summary>
        /// The project's name
        /// </summary>
        public string? Name { get; set; }

        public string? SaveFileDirectory { get; set; }

        private readonly ProjectSave _projectSave;
        private readonly INotifier _notifier;

        public UiSonProjectView(ProjectSave projectSave, INotifier notifier, string? name = "New Project", string? saveFileDirectory = null)
        {
            _projectSave = projectSave ?? throw new ArgumentNullException(nameof(projectSave));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            Name = name;
            SaveFileDirectory = saveFileDirectory;

            Load();
        }

        private void Load()
        {
            if (SaveFileDirectory != null)
            {
                foreach (var assembly in _projectSave.Assemblies)
                {
                    AddAssembly(Path.Combine(SaveFileDirectory, assembly));
                }

                // make a new element for each file
                foreach (var elementManager in ElementManagers)
                {
                    var elementDir = Path.Combine(SaveFileDirectory, elementManager.ElementName);

                    if (Directory.Exists(elementDir))
                    {
                        foreach (var file in Directory.GetFiles(elementDir))
                        {
                            if (Path.GetExtension(file) == elementManager.Extension)
                            {
                                var intialValue = JsonSerializer.Deserialize(File.ReadAllText(file),
                                                                             elementManager.ElementType,
                                                                             new JsonSerializerOptions() { IncludeFields = true });

                                if (intialValue == null)
                                {
                                    _notifier.Notify($"Unable to open {file}", $"Open {elementManager.ElementName}");
                                }
                                else
                                {
                                    elementManager.NewElement(Path.GetFileNameWithoutExtension(file), intialValue);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool Save()
        {
            if (!string.IsNullOrWhiteSpace(SaveFileDirectory) && !string.IsNullOrWhiteSpace(Name))
            {
                _projectSave.Assemblies = _assemblies.Select(x => x.Path).ToList();

                File.WriteAllText(Path.Combine(SaveFileDirectory, Name + ".uis"),
                                  JsonSerializer.Serialize(_projectSave));

                foreach (var elementManager in ElementManagers)
                {
                    elementManager.Save(SaveFileDirectory);
                }

                HasUnsavedChanges = false;

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the assembly from the given path to the project
        /// </summary>
        /// <param name="path">File path to the assembly</param>
        public void AddAssembly(string path)
        {
            var assemblyView = _assemblies.FirstOrDefault(x => x.Path == path);
            if (assemblyView == null)
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom(path);
                }
                catch
                {
                    _notifier.Notify($"Unable to load assembly from {path}", "Add Assembly Error");
                }

                if (assembly != null)
                {
                    var newAssemblyView = new AssemblyView(assembly,
                                                           new Dictionary<string, string[]>(),
                                                           _notifier);

                    _assemblies.Add(newAssemblyView);

                    HasUnsavedChanges = true;
                    OnPropertyChanged(nameof(Assemblies));
                    OnPropertyChanged(nameof(ElementManagers));
                    OnPropertyChanged(nameof(StringArrays));
                }
            }
        }

        /// <summary>
        /// Removes an assembly from the project
        /// </summary>
        /// <param name="assembly"></param>
        public void RemoveAssembly(IAssemblyView assembly)
        {
            _assemblies.Remove(assembly);

            OnPropertyChanged(nameof(Assemblies));
            OnPropertyChanged(nameof(ElementManagers));
            OnPropertyChanged(nameof(StringArrays));
            HasUnsavedChanges = true;
        }
    }
}
