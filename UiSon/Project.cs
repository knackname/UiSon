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
    public class Project : NPCBase
    {
        public string LogoPath => _projectSave.LogoPath;
        public string Description => _projectSave.Description;

        public IEnumerable<AssemblyVM> Assemblies => _assemblies;
        private ObservableCollection<AssemblyVM> _assemblies = new ObservableCollection<AssemblyVM>();

        public IEnumerable<ElementManager> ElementManagers => _elementManagers;
        private ObservableCollection<ElementManager> _elementManagers = new ObservableCollection<ElementManager>();

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

        private ProjectSave _projectSave;

        private TabControl _controller;
        private ElementFactory _elementFactory;

        public Project(ProjectSave projectSave, TabControl controller)
        {
            _projectSave = projectSave ?? throw new ArgumentNullException(nameof(projectSave));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _elementFactory = new ElementFactory(_elementManagers);//move out
        }

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

        public void RemoveAssembly(AssemblyVM assembly)
        {
            _assemblies.Remove(assembly);
        }

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
