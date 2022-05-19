// UiSon, by Cameron Gale 2021

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
        public string Path => _assembly.Location;

        public IEnumerable<IElementManager> ElementManagers => _elementManagers;
        private List<ElementManager> _elementManagers = new List<ElementManager>();

        public IEnumerable<KeyValuePair<string, string[]>> StringArrays => _stringArrays;
        private readonly Dictionary<string, string[]> _stringArrays;

        private readonly Assembly _assembly;
        private readonly INotifier _notifier;

        public AssemblyView(Assembly assembly,
                            Dictionary<string, string[]> stringArrays,
                            INotifier notifier)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _stringArrays = stringArrays ?? throw new ArgumentNullException(nameof(stringArrays));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));

            Load();
        }

        /// <summary>
        /// Refreshes element managers
        /// </summary>
        private void Load()
        {
            foreach (Type type in _assembly.GetTypes())
            {
                // check for string arrays
                var strArrays = type.GetCustomAttributes(typeof(UiSonArrayAttribute)) as IEnumerable<UiSonArrayAttribute>;

                if (strArrays != null)
                {
                    foreach (var array in strArrays)
                    {
                        if (array.Name != null)
                        {
                            if (!_stringArrays.ContainsKey(array.Name))
                            {
                                _stringArrays.Add(array.Name, null);
                            }

                            _stringArrays[array.Name] = array.Array;
                        }
                    }
                }

                var elementAtt = type.GetCustomAttribute<UiSonElementAttribute>();

                if (elementAtt != null)
                {
                    if (type.IsValueType || type.GetConstructor(new Type[] { }) != null)
                    {
                        var newElementManager = new ElementManager(type, elementAtt);
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
