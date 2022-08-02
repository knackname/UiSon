// UiSon, by Cameron Gale 2022

using System.Windows;
using UiSon.Notify.Interface;
using UiSon.Resource;
using UiSon.ViewModel;
using UiSon.ViewModel.Interface;

namespace UiSon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string _version = "Beta 1.1.1";

        public MainWindow(DynamicResourceDictionary skinDict, INotifier notifier, string? filePath)
        {
            this.Title = $"UiSon {_version}";

            InitializeComponent();

            UiSonUiContainer.Child = new UiSonUi(notifier,
                                                 skinDict,
                                                 new EditorModuleFactory(new ModuleTemplateSelector(),
                                                                         new ClipBoardManager(notifier),
                                                                         notifier),
                                                 filePath);
        }
    }
}
