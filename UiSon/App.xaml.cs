// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UiSon.Notify;
using UiSon.ViewModel;

namespace UiSon
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Dictionary<string, string> DefaultSkins = new Dictionary<string, string>()
        {
            { "Light", "pack://application:,,,/UiSon.Resource;Component/Skins/LightSkin.xaml" },
            //{ "Dark", "pack://application:,,,/UiSon.Resource;Component/Skins/DarkSkin.xaml" }
        };

        /// <summary>
        /// Startup handler
        /// </summary>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var skinDict = new DynamicResourceDictionary();

            foreach (var skin in DefaultSkins)
            {
                skinDict.AddSource(skin.Key, new Uri(skin.Value));
            }

            Resources.MergedDictionaries.Add(skinDict);

            new MainWindow(skinDict, new Notifier(), (e.Args.Length == 1 && Directory.Exists(e.Args[0])) ? e.Args[0] : null).Show();
        }
    }
}
