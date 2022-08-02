using System.Collections.Generic;
using System.Linq;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class HistoryManager
    {
        private struct HistoryEntry
        {
            public object OldValue;
            public object NewValue;
            public IValueEditorModule Source;
        }

        public bool IsCollecting = true;

        private List<HistoryEntry> undoEntries = new List<HistoryEntry>();
        private Stack<HistoryEntry> redoEntries = new Stack<HistoryEntry>();

        public void Undo()
        {
            if (undoEntries.Any())
            {
                var top = undoEntries.FirstOrDefault();
                undoEntries.RemoveAt(0);

                top.Source.Value = top.OldValue;

                redoEntries.Push(top);
            }

        }

        public void Redo()
        {
            if (redoEntries.Any())
            {
                var top = redoEntries.Pop();

                top.Source.Value = top.NewValue;

                undoEntries.Add(top);
            }
        }

        public void Act(IValueEditorModule source, object? newValue)
        {
            if (IsCollecting)
            {
                undoEntries.Add(new HistoryEntry() { OldValue = source.Value, NewValue = newValue, Source = source });

                redoEntries.Clear();

                while (undoEntries.Count > 30)
                {
                    undoEntries.RemoveAt(undoEntries.Count - 1);
                }
            }
        }
    }
}
