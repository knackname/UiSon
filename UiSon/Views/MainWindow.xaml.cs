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
        public string ProjectName => _project?.Name;

        public string LogoPath => _project?.LogoPath;

        public string Description => _project?.Description;

        public IEnumerable<AssemblyVM> Assemblies => _project?.Assemblies;

        public IEnumerable<ElementManager> ElementManagers => _project?.ElementManagers;

        private Project _project;

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();

            NewProject();

            this.Closing += OnClosing;

            Refresh();
        }

        private void NewProject()
        {
            if (_project?.UnsavedChanges ?? false)
            {
                // prompt save
            }

            _project = new Project(new ProjectSave(), this.TabControl);
            _project.PropertyChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(ProjectName));
                OnPropertyChanged(nameof(ElementManagers));
            };

            Refresh();
        }

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

        private void Refresh()
        {
            OnPropertyChanged(nameof(ElementManagers));
            OnPropertyChanged(nameof(Assemblies));
            OnPropertyChanged(nameof(ProjectName));
            OnPropertyChanged(nameof(LogoPath));
            OnPropertyChanged(nameof(Description));
        }

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

        private void NewProject_Click(object sender, RoutedEventArgs e) => NewProject();

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

        private void Save_Click(object sender, RoutedEventArgs e) => Save();

        private void AddAssembly_Click(object sender, RoutedEventArgs e) => AddAssembly();

        #endregion

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayElementEditor((sender as DataGrid).SelectedValue as ElementVM);
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
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
