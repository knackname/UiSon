// UiSon, by Cameron Gale 2022

namespace UiSon.Notify.Interface
{
    /// <summary>
    /// Displayes messages to t he user
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Sends an notification
        /// </summary>
        /// <param name="category">The notifications category, used for grouping</param>
        /// <param name="caption"></param>
        void Notify(string text, string caption);

        /// <summary>
        /// Starts a notification cashe. When the notifier has a cashe, notifications will not be displayed until the cashe ends.
        /// Every StartCashe call should be paired with an eventual call to EndCashe
        /// </summary>
        public void StartCashe();

        /// <summary>
        /// Ends a notification cashe and displays the notifications therein.
        /// A Start cashe must be called before this.
        /// </summary>
        public void EndCashe();
    }
}
