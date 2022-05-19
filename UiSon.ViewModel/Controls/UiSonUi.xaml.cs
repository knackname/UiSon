// UiSon, by Cameron Gale 2022

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Command;
using UiSon.Element;
using UiSon.Notify.Interface;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UiSonUi : UserControl, INotifyPropertyChanged
    {
        public string? ProjectName => _project.HasUnsavedChanges ? _project.Name + "*": _project.Name ;
        public string Description => _project.Description;
        public bool AllowAssemblyMod => _project.AllowAssemblyMod;
        public IEnumerable<IAssemblyView> Assemblies => _project.Assemblies;
        public IEnumerable<IElementManager> ElementManagers => _project.ElementManagers;
        public IEnumerable<string> SkinOptions => _skinDict.Keys;

        public IUiSonProject Project
        {
            get => _project;
            set
            {
                if (_project != null)
                {
                    _project.PropertyChanged -= OnProjectPropertyChanged;
                }

                _project = value;

                _editorModuleFactory.Project = _project;
                _skinDict.ChangeSource(_project.Skin);

                OnPropertyChanged(nameof(ProjectName));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(AllowAssemblyMod));
                OnPropertyChanged(nameof(Assemblies));
                OnPropertyChanged(nameof(ElementManagers));

                _project.PropertyChanged += OnProjectPropertyChanged;
            }
        }
        private IUiSonProject _project;

        private readonly INotifier _notifier;
        private readonly DynamicResourceDictionary _skinDict;
        private readonly EditorModuleFactory _editorModuleFactory;

        /// <summary>
        /// Constructor
        /// </summary>
        public UiSonUi(IUiSonProject project,
                       INotifier notifier,
                       DynamicResourceDictionary skinDict,
                       EditorModuleFactory editorModuleFactory,
                       IEnumerable<ElementView> initialViews = null,
                       ElementView selectedView = null)
        {
            _editorModuleFactory = editorModuleFactory ?? throw new ArgumentNullException(nameof(editorModuleFactory));
            _skinDict = skinDict ?? throw new ArgumentNullException(nameof(skinDict));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            DataContext = this;
            InitializeComponent();

            if (initialViews != null)
            {
                IElementEditorTab selectedTab = null;

                foreach (var view in initialViews)
                {
                    var newTab = _editorModuleFactory.MakeElementEditorTab(view, this.TabControl);

                    if (view == selectedView)
                    {
                        selectedTab = newTab;
                    }

                    this.TabControl.Items.Add(newTab);
                }

                this.TabControl.SelectedItem = selectedTab;
            }
        }

        private void OnProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiSonProject.Name):
                    OnPropertyChanged(nameof(ProjectName));
                    break;
                case nameof(IUiSonProject.Description):
                    OnPropertyChanged(nameof(Description));
                    break;
                case nameof(IUiSonProject.AllowAssemblyMod):
                    OnPropertyChanged(nameof(AllowAssemblyMod));
                    break;
                case nameof(IUiSonProject.Assemblies):
                    OnPropertyChanged(nameof(Assemblies));
                    break;
                case nameof(IUiSonProject.ElementManagers):
                    OnPropertyChanged(nameof(ElementManagers));
                    break;
                case nameof(IUiSonProject.HasUnsavedChanges):
                    OnPropertyChanged(nameof(ProjectName));
                    break;
            }
        }

        /// <summary>
        /// Checks for unsaved changes and prompts save
        /// </summary>
        private void HandleUnsavedChanges()
        {
            if (_project.HasUnsavedChanges)
            {
                var result = MessageBox.Show("The current project has unsaved changes, would you like to save?",
                                             "Unsaved Changes",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Save();
                }
            }
        }

        /// <summary>
        /// Replaces the current project with a new default one
        /// </summary>
        private void NewProject()
        {
            HandleUnsavedChanges();
            var newProject = new UiSonProjectView(new ProjectSave(), _notifier);
            newProject.Skin = _skinDict.Current;
            Project = newProject;
        }

        /// <summary>
        /// Propmts the user for a file and opens it as a project
        /// </summary>
        private void OpenProject()
        {
            HandleUnsavedChanges();

            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".uis";
            dlg.Filter = "UiSon Project|*.uis";
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() ?? false)
            {
                var projectSave = JsonSerializer.Deserialize<ProjectSave>(File.ReadAllText(dlg.FileName));

                if (projectSave != null)
                {
                    Project = new UiSonProjectView(projectSave,
                                                   _notifier,
                                                   Path.GetFileNameWithoutExtension(dlg.FileName),
                                                   Path.GetDirectoryName(dlg.FileName));
                }
                else
                {
                    _notifier.Notify($"{dlg.FileName} could not be read.", "Open Failed");
                }
            }
        }

        /// <summary>
        /// Opens the view in a <see cref="IElementEditorTab"/>
        /// </summary>
        /// <param name="view">The element view to open</param>
        private void OpenEditorTab(ElementView view)
        {
            // if the view is already open focus it
            foreach (var tab in this.TabControl.Items)
            {
                if (tab is IElementEditorTab elementEditorTab
                    && elementEditorTab.View == view)
                {
                    this.TabControl.SelectedItem = tab;
                    return;
                }
            }

            // if it wasn't, open it in a new tab
            var newTab = _editorModuleFactory.MakeElementEditorTab(view, this.TabControl);

            this.TabControl.Items.Add(newTab);
            this.TabControl.SelectedItem = newTab;
        }

        /// <summary>
        /// Saves the current project
        /// </summary>
        private void Save() 
        {
            if (_project.SaveFileDirectory == null)
            {
                SaveAs();
            }
            else
            {
                _project.Save();
            }
        }

        /// <summary>
        /// Prompts the user for a new location for the project and saves it
        /// </summary>
        private void SaveAs()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = ".uis";
            dlg.Filter = "UiSon Project|*.uis";

            if (dlg.ShowDialog() ?? false)
            {
                _project.Name = Path.GetFileNameWithoutExtension(dlg.FileName);
                _project.SaveFileDirectory = Path.GetDirectoryName(dlg.FileName);
                _project.Save();
            }
        }

        private void RemoveElementManager()
        {

        }

        private void RemoveElementView(ElementView view)
        {
            foreach (var tab in this.TabControl.Items)
            {
                if (tab is IElementEditorTab elementEditorTab
                    && elementEditorTab.View == view)
                {
                    this.TabControl.Items.Remove(view);
                }
            }

            view.Manager.RemoveElement(view);
        }

        /// <summary>
        /// Prompts the user for an assembly and adds it to the project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAssembly_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dll";
            dlg.Filter = "C# Library|*.dll";
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() ?? false)
            {
                _project.AddAssembly(dlg.FileName);
            }
        }

        /// <summary>
        /// Removes the assembly from the project
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAssembly_Click(object sender, RoutedEventArgs e)
        {
            var assemblyView = (AssemblyView)((Button)sender).DataContext;

            foreach (var tab in this.TabControl.Items)
            {
                if (tab is IElementEditorTab elementEditorTab
                    && assemblyView.ElementManagers.Any(x => x.Elements.Any(y => x == elementEditorTab.View)))
                {
                    this.TabControl.Items.Remove(tab);
                }
            }

            Project.RemoveAssembly(assemblyView);
        }

        private void Element_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var elementView = ((DataGrid)sender).SelectedItem as ElementView;

            if (elementView != null)
            {
                OpenEditorTab(elementView);
            }
        }

        private void ElementManagerAdd_Click(object sender, RoutedEventArgs e)
        {
            ((ElementManager)((Button)sender).DataContext).NewElement();
        }

        private void ElementManagerImport_Click(object sender, RoutedEventArgs e)
        {
            var elementManager = (ElementManager)((Button)sender).DataContext;

            var dlg = new OpenFileDialog();
            dlg.DefaultExt = elementManager.Extension;
            dlg.Filter = $"{elementManager.ElementName}|*{elementManager.Extension}";
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() ?? false)
            {
                var intialValue = JsonSerializer.Deserialize(File.ReadAllText(dlg.FileName),
                                                             elementManager.ElementType,
                                                             new JsonSerializerOptions() { IncludeFields = true });

                if (intialValue == null)
                {
                    _notifier.Notify($"Unable to open {dlg.FileName}", $"Open {elementManager.ElementName}");
                }
                else
                {
                    elementManager.NewElement(Path.GetFileNameWithoutExtension(dlg.FileName), intialValue);
                }
            }
        }

        private void ElementManagerRemove_Click(object sender, RoutedEventArgs e)
        {
            var view = (ElementView)((Button)sender).DataContext;

            foreach (var tab in this.TabControl.Items)
            {
                if (tab is IElementEditorTab elementEditorTab
                    && elementEditorTab.View == view)
                {
                    this.TabControl.Items.Remove(view);
                }
            }

            view.Manager.RemoveElement(view);
        }

        #region Commands

        public ICommand NewCommand => new UiSonActionCommand((s) => NewProject());

        public ICommand OpenCommand => new UiSonActionCommand((s) => OpenProject());

        public ICommand SaveCommand => new UiSonActionCommand((s) => Save());

        public ICommand SaveAsCommand => new UiSonActionCommand((s) => SaveAs());

        public ICommand ChangeSkinCommand => new UiSonActionCommand((s) => _skinDict.ChangeSource(s as string));

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
