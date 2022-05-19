// UiSon, by Cameron Gale 2022

using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using UiSon.Element;
using UiSon.Notify.Interface;
using UiSon.View;
using UiSon.ViewModel;
using UiSon.ViewModel.Interface;

namespace UiSon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string _version = "Beta 1.1";

        public MainWindow(DynamicResourceDictionary skinDict, INotifier notifier, string? filePath)
        {
            this.Title = $"UiSon {_version}";
            
            ProjectSave? projectSave = null;

            if (filePath != null)
            {
                var serializedSave = File.ReadAllText(filePath);

                if (serializedSave != null)
                {
                    projectSave = JsonSerializer.Deserialize<ProjectSave>(serializedSave);
                }

                if (projectSave == null)
                {
                    notifier.Notify($"Unable to open {filePath}", "Open Failed");
                    projectSave = new ProjectSave();
                }

                // custom skins
                var directory = Path.GetDirectoryName(filePath);

                foreach (var skin in projectSave.CustomSkins)
                {
                    if (!string.IsNullOrWhiteSpace(skin.Value))
                    {
                        var skinPath = Path.Combine(directory, skin.Value);

                        try
                        {
                            var uri = new Uri(skinPath);
                            skinDict.AddSource(skin.Key, uri);
                        }
                        catch
                        {
                            notifier.Notify($"Unable to open skin {skin.Key} from {skinPath}", "Skin Failed");
                        }
                    }
                }
            }
            else
            {
                projectSave = new ProjectSave();
            }

            skinDict.ChangeSource(projectSave.Skin);

            InitializeComponent();

            UiSonUiContainer.Child = new UiSonUi(new UiSonProjectView(projectSave, notifier),
                                                 notifier,
                                                 skinDict,
                                                 new EditorModuleFactory(notifier, new ModuleTemplateSelector()));
        }
    }
}
