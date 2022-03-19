// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Windows;
using UiSon.Notify.Interface;

namespace UiSon.Notify
{
    /// <summary>
    /// Displays messages to the user
    /// </summary>
    public class Notifier : INotifier
    {
        private Stack<Dictionary<string, List<string>>> Cashes = new Stack<Dictionary<string, List<string>>>();

        public void Notify(string text, string caption)
        {
            if (Cashes.Count == 0)
            {
                ShowMessageBox(text, caption);
            }
            else
            {
                var cashe = Cashes.Peek();

                if (!cashe.ContainsKey(caption))
                {
                    cashe.Add(caption, new List<string>());
                }

                cashe[caption].Add(text);
            }
        }

        /// <summary>
        /// Starts a notification cashe. When the notifier has a cashe, notifications will not be displayed until the cashe ends.
        /// Every StartCashe call should be paired with an eventual call to EndCashe
        /// </summary>
        public void StartCashe() => Cashes.Push(new Dictionary<string, List<string>>());

        /// <summary>
        /// Ends a notification cashe and displays the notifications therein.
        /// A Start cashe must be called before this.
        /// </summary>
        public void EndCashe()
        {
            if (Cashes.Count == 0)
            {
                throw new Exception("EndCashe called without a cashe exsisting");
            }

            foreach (var category in Cashes.Pop())
            {
                ShowMessageBox(string.Join(", ", category.Value), category.Key);
            }
        }

        /// <summary>
        /// Displays a message box to the user
        /// </summary>
        /// <param name="text"></param>
        /// <param name="caption"></param>
        private void ShowMessageBox(string text, string caption) => MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
