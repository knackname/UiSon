// UiSon, by Cameron Gale 2021

using System;
using System.Windows;
using System.Text.Json;
using System.IO;
using System.ComponentModel;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UiSon.Views;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.ViewModel;

namespace UiSon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        /// <summary>
        /// Name of the project
        /// </summary>
        public string ProjectName => _project?.Name;

        /// <summary>
        /// Path to the project's logo
        /// </summary>
        public string LogoPath => _project?.LogoPath;

        /// <summary>
        /// The project's description
        /// </summary>
        public string Description => _project?.Description;

        /// <summary>
        /// The project's assemblies
        /// </summary>
        public IEnumerable<AssemblyVM> Assemblies => _project?.Assemblies;

        /// <summary>
        /// The project's element managers
        /// </summary>
        public IEnumerable<ElementManager> ElementManagers => _project?.ElementManagers;

        /// <summary>
        /// The current project
        /// </summary>
        private Project _project;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();

            //the project needs the TabControl, so we make it after inition wpf stuff
            NewProject();

            this.Closing += OnClosing;

            Refresh();
        }

        /// <summary>
        /// Opens a clean new project
        /// </summary>
        private void NewProject()
        {
            if (_project?.UnsavedChanges ?? false)
            {
                // prompt save
            }

            this.TabControl.Items.Clear();

            _project = new Project(new ProjectSave(), this.TabControl);
            _project.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(ProjectName));
                OnPropertyChanged(nameof(ElementManagers));
            };

            Refresh();
        }

        /// <summary>
        /// Opens a prompt for the user to save the project
        /// </summary>
        private void Save()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = ".uis";
            dlg.Filter = "UiSon Project|*.uis";

            if (dlg.ShowDialog() ?? false)
            {
                _project.Save(dlg.FileName);
            }
        }

        /// <summary>
        /// Refreshes for the UI
        /// </summary>
        private void Refresh()
        {
            OnPropertyChanged(nameof(ElementManagers));
            OnPropertyChanged(nameof(Assemblies));
            OnPropertyChanged(nameof(ProjectName));
            OnPropertyChanged(nameof(LogoPath));
            OnPropertyChanged(nameof(Description));
        }

        /// <summary>
        /// Opens or focuses a tab for the target element
        /// </summary>
        /// <param name="element">The target element</param>
        private void DisplayElementEditor(ElementVM element)
        {
            if (element != null)
            {
                foreach (var tab in this.TabControl.Items)
                {
                    if (tab is ElementEditor elementEditor
                        && elementEditor.ElementVM == element)
                    {
                        this.TabControl.SelectedItem = tab;
                        return;
                    }
                }

                var girlEditor = new ElementEditor(element, this.TabControl);

                this.TabControl.Items.Add(girlEditor);

                this.TabControl.SelectedIndex = this.TabControl.Items.Count - 1;
            }
        }

        #region Assemblies

        /// <summary>
        /// Opens a prompt to add an assembly to the project
        /// </summary>
        public void AddAssembly()
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

        #endregion

        #region Event Hadlers

        #region Menu Items

        /// <summary>
        /// Handler for newProject menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewProject_Click(object sender, RoutedEventArgs e) => NewProject();

        /// <summary>
        /// Handler for OpenProject manu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenProject_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".uis";
            dlg.Filter = "UiSon Project|*.uis";
            dlg.CheckFileExists = true;

            if (dlg.ShowDialog() ?? false)
            {
                try
                {
                    NewProject();
                    _project.Load(dlg.FileName);
                    Refresh();
                }
                catch (Exception ex)
                {
                    // noltify currupt file
                }
            }
        }

        /// <summary>
        /// Handler for save menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e) => Save();

        /// <summary>
        /// Handler for add assembly menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAssembly_Click(object sender, RoutedEventArgs e) => AddAssembly();

        #endregion

        /// <summary>
        /// Handler for double clicking on an item in one of the element manager displays
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayElementEditor((sender as DataGrid).SelectedValue as ElementVM);
        }

        /// <summary>
        /// Prompts save if nessisary on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClosing(object sender, CancelEventArgs e)
        {
            // prompt save
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
