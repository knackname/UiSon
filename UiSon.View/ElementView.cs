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
    public class ElementView : IElementView
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
                    && !string.IsNullOrWhiteSpace(value)
                    && value != "null")
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

                    // TODO, I don;t think this is needed, a regular opc should do
                    PropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<string>(nameof(Name), old, _name));
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        private string _name = string.Empty;

        /// <summary>
        /// The name this element was last saved as, 
        /// </summary>
        private string _savedName;

        /// <inheritdoc/>
        public object? Value => _view.Value;

        /// <inheritdoc/>
        public IElementManager Manager => _manager;
        private readonly ElementManager _manager;

        /// <inheritdoc/>
        public Type ElementType => _manager.ElementType;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, IUiValueView> TagNameToView { get; private set; }

        /// <inheritdoc/>
        public IUiValueView MainView => _view;
        private readonly IUiValueView _view;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name, will be made unique.</param>
        /// <param name="view">The view representing the element.</param>
        /// <param name="identifiers">Member views with identifier tags for other views to subscribe to.</param>
        /// <param name="manager">This element's manager.</param>
        public ElementView(string name,
                           IUiValueView view,
                           IReadOnlyDictionary<string, IUiValueView> tagNameToView,
                           ElementManager manager)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentException(nameof(name)); }
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;

            TagNameToView = tagNameToView ?? throw new ArgumentNullException(nameof(tagNameToView));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));

            // set the saved name, then set the Name which will be made unique in its setter
            _savedName = name;
            Name = _savedName;
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            { 
                case nameof(IUiValueView.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        /// <summary>
        /// Saves this view as a json.
        /// </summary>
        /// <param name="saveDirectory">The directory to save to.</param>
        public void Save(string saveDirectory)
        {
            // delete the old file if its name isn't used by any element
            if (!_manager.Elements.Any(x => x.Name == _savedName))
            {
                File.Delete(Path.Combine(saveDirectory, _savedName + _manager.Extension));
            }

            _savedName = Name;

            File.WriteAllText(Path.Combine(saveDirectory, Name + _manager.Extension),
                              JsonSerializer.Serialize(Value, new JsonSerializerOptions() { IncludeFields = true }));
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
