// UiSon, by Cameron Gale 2022

using System;
using System.Text.Json;
using System.Windows;
using UiSon.Notify.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class ClipBoardManager
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions() { IncludeFields = true };

        private object? _value;


        private readonly INotifier _notifier;
        public ClipBoardManager(INotifier notifier)
        {
            _notifier = notifier;
        }

        public void Copy(IValueEditorModule source)
        {
            _value = source?.Value;
            Clipboard.SetText(JsonSerializer.Serialize(_value, options));
        }

        public void Paste(IValueEditorModule source)
        {
            if (source != null)
            {
                try
                {
                    source.Value = JsonSerializer.Deserialize(Clipboard.GetText(), source.ValueType, options);
                }
                catch (Exception e)
                {
                    _notifier.Notify(e.ToString(), "Paste Failed");
                }

            }
        }
    }
}
