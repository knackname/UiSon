// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using UiSon.Event;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A user creatable element
    /// </summary>
    public class ElementView : INotifyPropertyChanged, IElementView
    {
        /// <summary>
        /// Unique name for the element
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name
                    && !string.IsNullOrWhiteSpace(value))
                {
                    // make name unique
                    if (_manager.Elements.Any(x => x.Name == value))
                    {
                        int i = 2;
                        while (_manager.Elements.Any(x => x.Name == $"{value} {i}"))
                        {
                            i++;
                        }
                        value = $"{value} {i}";
                    }

                    var old = _name;
                    _name = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<string>(nameof(Name), old, _name));
                }
            }
        }
        private string _name;

        /// <summary>
        /// The name this element was last saved as, 
        /// </summary>
        private string _savedName;

        public object? Value => _value;
        private object? _value;

        public ElementManager Manager => _manager;
        private readonly ElementManager _manager;

        public Type ElementType => _manager.ElementType;

        public ElementView(string name, object? value, ElementManager manager)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException(nameof(name)); }
            _value = value ?? throw new ArgumentNullException(nameof(value));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));

            // set the saved name, then set the Name which will be made unique in its setter
            _savedName = name;
            Name = _savedName;
        }

        public void Save(string path)
        {
            // delete the old file if its name isn't used by any element
            if (!_manager.Elements.Any(x => x.Name == _savedName))
            {
                File.Delete(Path.Combine(path, _savedName + _manager.Extension));
            }

            _savedName = Name;

            File.WriteAllText(Path.Combine(path, Name + _manager.Extension),
                              JsonSerializer.Serialize(_value, new JsonSerializerOptions() { IncludeFields = true }));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
