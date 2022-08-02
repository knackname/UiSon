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
using UiSon.Resource;
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
        private List<IElementEditorTab> _tabItems = new List<IElementEditorTab>();

        /// <summary>
        /// The current project
        /// </summary>
        private IUiSonProject Project
        {
            get => _project;
            set
            {
                if (_project != null)
                {
                    _project.PropertyChanged -= OnProjectPropertyChanged;

                    // close exsisting stuff
                    var closingTabs = new List<IElementEditorTab>();

                    foreach (var assemblyView in _project.Assemblies)
                    {
                        foreach (var manager in assemblyView.ElementManagers)
                        {
                            foreach (var element in manager.Elements)
                            {
                                foreach (var tab in this.TabControl.Items)
                                {
                                    if (tab is IElementEditorTab elementEditorTab
                                        && elementEditorTab.View == element)
                                    {
                                        closingTabs.Add(elementEditorTab);
                                    }
                                }
                            }
                        }
                    }

                    foreach (var tab in closingTabs)
                    {
                        CloseTab(tab);
                    }
                }

                // new project
                _project = value;

                // skins
                foreach (var skin in _project.ProjectSave.CustomSkins)
                {
                    if (!string.IsNullOrWhiteSpace(skin.Value))
                    {
                        var skinPath = Path.Combine(_project.SaveFileDirectory ?? string.Empty, skin.Value);

                        try
                        {
                            var uri = new Uri(skinPath);
                            _skinDict.AddSource(skin.Key, uri);
                        }
                        catch
                        {
                            _notifier.Notify($"Unable to open skin {skin.Key} from {skinPath}", "Skin Failed");
                        }
                    }
                }

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
        public UiSonUi(ProjectSave projectSave,
                       INotifier notifier,
                       DynamicResourceDictionary skinDict,
                       EditorModuleFactory editorModuleFactory)
        {
            _editorModuleFactory = editorModuleFactory ?? throw new ArgumentNullException(nameof(editorModuleFactory));
            _skinDict = skinDict ?? throw new ArgumentNullException(nameof(skinDict));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            Project = new UiSonProjectView(projectSave, _notifier);

            DataContext = this;
            InitializeComponent();

            this.TabControl.DataContext = _tabItems;
        }

        private void OnProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
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
                try
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
                catch (Exception e)
                {
                    _notifier.Notify(e.ToString(), "Open Failed");
                }
            }
        }

        /// <summary>
        /// Opens the view in a <see cref="IElementEditorTab"/>
        /// </summary>
        /// <param name="view">The element view to open</param>
        private void OpenEditorTab(IElementView view)
        {
            if (view == null) { return; }

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

            // clear tab control binding
            this.TabControl.DataContext = null;

            // add new tab
            var newTab = new ElementEditorTab(view, _editorModuleFactory.MakeEditorModule(view.MainView), this);
            _tabItems.Add(newTab);

            // bind tab control
            this.TabControl.DataContext = _tabItems;

            // select newly added tab item
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

        private void RemoveElement(IElementView view)
        {
            var closingTabs = new List<IElementEditorTab>();

            foreach (var tab in this.TabControl.Items)
            {
                if (tab is IElementEditorTab elementEditorTab
                    && elementEditorTab.View == view)
                {
                    closingTabs.Add(elementEditorTab);
                }
            }

            foreach (var tab in closingTabs)
            {
                CloseTab(tab);
            }

            view.Manager.RemoveElement(view);
        }

        private void ChangeSkin(string name)
        {
            _skinDict.ChangeSource(name);

            var selectedView = (this.TabControl.SelectedItem as IElementEditorTab)?.View;

            // clear tab control binding
            this.TabControl.DataContext = null;

            // reopen all the editor tabs, unfortunatly nessisary since 
            // the converters don't update dynamically
            
            var openElements = new List<IElementView>();

            foreach (var tab in _tabItems)
            {
                if (tab is IElementEditorTab elementEditorTab)
                {
                    openElements.Add(elementEditorTab.View);
                }
            }

            _tabItems.Clear();

            foreach (var view in openElements)
            {
                // add new tab
                var newTab = new ElementEditorTab(view, _editorModuleFactory.MakeEditorModule(view.MainView), this);
                _tabItems.Add(newTab);
            }

            // bind tab control
            this.TabControl.DataContext = _tabItems;

            OpenEditorTab(selectedView);
        }

        public void CloseTab(IElementEditorTab tab)
        {
            // get selected tab
            var selectedTab = this.TabControl.SelectedItem as IElementEditorTab;

            // clear tab control binding
            this.TabControl.DataContext = null;

            tab.Destroy();
            _tabItems.Remove(tab);

            // bind tab control
            this.TabControl.DataContext = _tabItems;

            // select previously selected tab. if that is removed then select first tab
            if (selectedTab == null || selectedTab.Equals(tab))
            {
                selectedTab = _tabItems.FirstOrDefault();
            }
            this.TabControl.SelectedItem = selectedTab;
        }

        #region Clicks

        private void ElementManagerRemove_Click(object sender, RoutedEventArgs e) => RemoveElement((ElementView)((Button)sender).DataContext);

        private void AddAssembly_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dll";
            dlg.Filter = "C# Library|*.dll";
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() ?? false)
            {
                _project.AddAssembly(string.Empty, dlg.FileName);
            }
        }

        private void RemoveAssembly_Click(object sender, RoutedEventArgs e)
        {
            var assemblyView = (AssemblyView)((Button)sender).DataContext;

            var closingTabs = new List<IElementEditorTab>();

            foreach (var manager in assemblyView.ElementManagers)
            {
                foreach (var element in manager.Elements)
                {
                    RemoveElement(element);
                }
            }

            Project.RemoveAssembly(assemblyView);
        }

        private void ElementOpen_Click(object sender, RoutedEventArgs e)
        {
            var elementView = ((Button)sender).DataContext as ElementView;

            if (elementView != null)
            {
                OpenEditorTab(elementView);
            }
        }

        private void ElementManagerAdd_Click(object sender, RoutedEventArgs e) => ((ElementManager)((Button)sender).DataContext).NewDefaultElement();

        private void ElementManagerImport_Click(object sender, RoutedEventArgs e)
        {
            var elementManager = (ElementManager)((Button)sender).DataContext;

            var dlg = new OpenFileDialog();
            dlg.DefaultExt = elementManager.Extension;
            dlg.Filter = $"{elementManager.ElementName}|*{elementManager.Extension}";
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() ?? false)
            {
                try
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
                        Project.ImportElement(elementManager, Path.GetFileNameWithoutExtension(dlg.FileName), intialValue);
                    }
                }
                catch (Exception ex)
                {
                    _notifier.Notify($"Exception thrown opening {dlg.FileName} - {ex}", $"Open {elementManager.ElementName}");
                }
            }
        }

        #endregion

        #region Commands

        public ICommand NewCommand => new UiSonActionCommand((s) => NewProject());

        public ICommand OpenCommand => new UiSonActionCommand((s) => OpenProject());

        public ICommand SaveCommand => new UiSonActionCommand((s) => Save());

        public ICommand SaveAsCommand => new UiSonActionCommand((s) => SaveAs());

        public ICommand ChangeSkinCommand => new UiSonActionCommand((s) => ChangeSkin(s as string));

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
