// UiSon, by Cameron Gale 2021

using System.ComponentModel;
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
    public class UiSonProjectView : NPCBase, IUiSonProject, IHaveProject
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler? ProjectChanged;

        /// <inheritdoc/>
        public string? Skin
        {
            get => _projectSave.Skin;
            set => _projectSave.Skin = value;
        }

        /// <inheritdoc/>
        public string Description => _projectSave.Description;

        /// <inheritdoc/>
        public bool AllowAssemblyMod => _projectSave.AllowAssemblyMod;

        /// <inheritdoc/>
        public IEnumerable<IAssemblyView> Assemblies => _assemblies.OrderBy(x => x.Path);
        private List<IAssemblyView> _assemblies = new List<IAssemblyView>();

        /// <inheritdoc/>
        public IEnumerable<IElementManager> ElementManagers => _assemblies.SelectMany(x => x.ElementManagers);

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, object[]>> Arrays => _assemblies.SelectMany(x => x.Arrays);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public string? Name { get; set; }

        /// <inheritdoc/>
        public string? SaveFileDirectory { get; set; }

        /// <inheritdoc/>
        public IUiSonProject Project => this;

        /// <inheritdoc/>
        public ProjectSave ProjectSave => _projectSave;
        private readonly ProjectSave _projectSave;

        private readonly INotifier _notifier;
        private readonly ViewFactory _factory;

        public UiSonProjectView(ProjectSave projectSave, INotifier notifier, string? name = "New Project", string? saveFileDirectory = null)
        {
            _projectSave = projectSave ?? throw new ArgumentNullException(nameof(projectSave));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _factory = new ViewFactory(this);

            Name = name;
            SaveFileDirectory = saveFileDirectory;

            Load();

            HasUnsavedChanges = false;
        }

        private void OnAssemblyPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Value":
                    HasUnsavedChanges = true;
                    break;
            }

        }

        private void Load()
        {
            if (SaveFileDirectory != null)
            {
                foreach (var assembly in _projectSave.Assemblies)
                {
                    AddAssembly(SaveFileDirectory, assembly);
                }

                var elementValuePairs = new List<KeyValuePair<IElementView, object>>();

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
                                    elementValuePairs.Add(new KeyValuePair<IElementView, object>(elementManager.NewElement(Path.GetFileNameWithoutExtension(file)), intialValue));
                                }
                            }
                        }
                    }
                }

                // now notify the project changed to allow the ref views to find their elements
                ProjectChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project)));

                // then init them all.
                foreach (var pair in elementValuePairs)
                {
                    pair.Key.MainView.SetValue(pair.Value);
                }
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void AddAssembly(string directory, string relativePath)
        {
            var fullPath = Path.Combine(directory, relativePath);

            if (!_assemblies.Any(x => x.Path == relativePath))
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom(fullPath);
                }
                catch
                {
                    _notifier.Notify($"Unable to load assembly from {fullPath}", "Add Assembly Error");
                }

                if (assembly != null)
                {
                    var newAssemblyView = new AssemblyView(assembly,
                                                           relativePath,
                                                           new Dictionary<string, object[]>(),
                                                           _notifier,
                                                           _factory);

                    newAssemblyView.PropertyChanged += OnAssemblyPropertyChanged;

                    _assemblies.Add(newAssemblyView);

                    HasUnsavedChanges = true;
                    OnPropertyChanged(nameof(Assemblies));
                    OnPropertyChanged(nameof(ElementManagers));
                    OnPropertyChanged(nameof(Arrays));
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveAssembly(IAssemblyView assembly)
        {
            assembly.PropertyChanged -= OnAssemblyPropertyChanged;
            _assemblies.Remove(assembly);

            OnPropertyChanged(nameof(Assemblies));
            OnPropertyChanged(nameof(ElementManagers));
            OnPropertyChanged(nameof(Arrays));
            HasUnsavedChanges = true;
        }

        /// <inheritdoc/>
        public void ImportElement(IElementManager elementManager, string name, object value)
        {
            var newElement = elementManager.NewElement(name);

            ProjectChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Project)));

            newElement.MainView.SetValue(value);
        }
    }
}
