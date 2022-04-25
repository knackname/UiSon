// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Command;
using UiSon.Notify.Interface;
using UiSon.ViewModel;

namespace UiSon
{
    /// <summary>
    /// An assembly loaded into UiSon with it's own defined elements
    /// </summary>
    public class AssemblyVM
    {
        /// <summary>
        /// Path to the assembly on disk, on save reroutes to relative path from save file location
        /// </summary>
        public string Path => _path;
        private string _path;

        /// <summary>
        /// Removes the vm from its parent collection
        /// </summary>
        public ICommand RemoveCommand => new UiSonActionCommand((s) => { _parent.RemoveAssembly(this); RemoveElements(); });

        private Project _parent;

        private Collection<ElementManager> _elementManagers;
        private List<ElementManager> _myElementManagers = new List<ElementManager>();
        private Dictionary<string, string[]> _stringArrays;

        private TabControl _controller;
        private EditorModuleFactory _elementFactory;
        private INotifier _notifier;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="elementManagers"></param>
        /// <param name="controller"></param>
        /// <param name="elementFactory"></param>
        public AssemblyVM(string path,
                         Project parent,
                         Collection<ElementManager> elementManagers,
                         TabControl controller,
                         Dictionary<string, string[]> stringArrays,
                         EditorModuleFactory elementFactory,
                         INotifier notifier)
        {
            _path = path;
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _elementManagers = elementManagers ?? throw new ArgumentNullException(nameof(elementManagers));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _elementFactory = elementFactory ?? throw new ArgumentNullException(nameof(elementFactory));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _stringArrays = stringArrays ?? throw new ArgumentNullException(nameof(stringArrays));

            RefreshElements();
        }

        /// <summary>
        /// Refreshes element managers
        /// </summary>
        public void RefreshElements()
        {
            var dll = Assembly.LoadFrom(_path);
            foreach (Type type in dll.GetTypes())
            {
                // check for string arrays
                var strArrays = type.GetCustomAttributes(typeof(UiSonStringArrayAttribute)) as IEnumerable<UiSonStringArrayAttribute>;

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

                foreach (var attribute in System.Attribute.GetCustomAttributes(type))
                {
                    if (attribute is UiSonElementAttribute ele)
                    {
                        if (!_elementManagers.Any(x => x.ElementName == type.Name))
                        {
                            // make sure it a parameteless constructor
                            if (type.GetConstructor(new Type[] { }) != null)
                            {
                                // create manager
                                var newElementManager = new ElementManager(type, _controller, _elementFactory, ele);
                                _elementManagers.Add(newElementManager);
                                _myElementManagers.Add(newElementManager);
                            }
                            else
                            {
                                _notifier.Notify($"{type} lacks a parameterless constructor.", $"Invalid {nameof(UiSonElementAttribute)}");
                            }
                        }
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all element managers and their elements
        /// </summary>
        private void RemoveElements()
        {
            foreach (var elementManager in _myElementManagers)
            {
                elementManager.Clear();
                _elementManagers.Remove(elementManager);
            }
        }
    }
}
