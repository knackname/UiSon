// UiSon, by Cameron Gale 2021

using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using UiSon.Attribute;
using UiSon.Notify.Interface;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// An assembly loaded into UiSon with its own defined elements
    /// </summary>
    public class AssemblyView : IAssemblyView, INotifyPropertyChanged
    {
        /// <summary>
        /// Path to the assembly on disk, on save reroutes to relative path from save file location
        /// </summary>
        public string Path => _path;
        private readonly string _path;

        public IEnumerable<IElementManager> ElementManagers => _elementManagers;
        private List<ElementManager> _elementManagers = new List<ElementManager>();

        public IEnumerable<KeyValuePair<string, object[]>> Arrays => _arrays;
        private readonly Dictionary<string, object[]> _arrays;

        private readonly Assembly _assembly;
        private readonly INotifier _notifier;
        private readonly ViewFactory _factory;

        public AssemblyView(Assembly assembly,
                            string path,
                            Dictionary<string, object[]> arrays,
                            INotifier notifier,
                            ViewFactory factory)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _path = path ?? throw new ArgumentNullException(nameof(path));
            _arrays = arrays ?? throw new ArgumentNullException(nameof(arrays));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

            Load();
        }

        private void OnElementManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Element":
                case nameof(ElementManager.Elements):
                    OnPropertyChanged("Value");
                    break;
            }
        }

        /// <summary>
        /// Refreshes element managers
        /// </summary>
        private void Load()
        {
            foreach (Type type in _assembly.GetTypes())
            {
                // check for string arrays
                var arrayAtts = type.GetCustomAttributes(typeof(UiSonArrayAttribute)) as IEnumerable<UiSonArrayAttribute>;

                if (arrayAtts != null)
                {
                    foreach (var arrayAtt in arrayAtts)
                    {


                        if (arrayAtt.Name != null)
                        {
                            _arrays.TryAdd(arrayAtt.Name,
                                           arrayAtt.JsonDeserializeType == null
                                            ? arrayAtt.Array.Select(x => x == null ? "null" : x).ToArray()
                                            : arrayAtt.Array.Select(x => JsonConvert.DeserializeObject(x?.ToString() ?? "null", arrayAtt.JsonDeserializeType) ?? "null").ToArray());
                        }
                    }
                }

                var elementAtt = type.GetCustomAttribute<UiSonElementAttribute>();

                if (elementAtt != null)
                {
                    if (type.IsValueType || type.GetConstructor(new Type[] { }) != null)
                    {
                        var newElementManager = new ElementManager(type, elementAtt, _factory);
                        newElementManager.PropertyChanged += OnElementManagerPropertyChanged;
                        _elementManagers.Add(newElementManager);
                    }
                    else
                    {
                        _notifier.Notify($"{type} lacks a parameterless constructor.", $"Invalid {nameof(UiSonElementAttribute)}");
                    }
                }
            }
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
