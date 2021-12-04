// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using UiSon.Attribute;
using UiSon.Commands;
using UiSon.ViewModel;
using UiSon.Views;

namespace UiSon
{
    /// <summary>
    /// Contains all of one type of UiSonElement
    /// </summary>
    public class ElementManager
    {
        /// <summary>
        /// Name of the element
        /// </summary>
        public string ElementName => _type.Name;

        /// <summary>
        /// Extension type of the elements file
        /// </summary>
        public string Extension => _ele.Extension;

        /// <summary>
        /// The elemetns of this manager's type
        /// </summary>
        public NotifyingCollection<ElementVM> Elements => _elementsVMs;
        private NotifyingCollection<ElementVM> _elementsVMs = new NotifyingCollection<ElementVM>();

        private UiSonElementAttribute _ele;
        private TabControl _controller;
        private Type _type;
        private ElementFactory _factory;
        private ConstructorInfo _constructor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Element type</param>
        /// <param name="constructor">Constructor for the type</param>
        /// <param name="controller">Tab controller for the main ui</param>
        /// <param name="factory"></param>
        public ElementManager(Type type, ConstructorInfo constructor, TabControl controller, ElementFactory factory)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));

            foreach (var attribute in System.Attribute.GetCustomAttributes(type))
            {
                if (attribute is UiSonElementAttribute ele)
                {
                    _ele = ele;
                    break;
                }
            }

            _elementsVMs.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var tab in _controller.Items)
                        {
                            if (tab is ElementEditor elementEditor
                                && (e.OldItems?.Contains(elementEditor.ElementVM) ?? false))
                            {
                                _controller.Items.Remove(elementEditor);
                                return;
                            }
                        }
                        break;
                }
            };
        }

        /// <summary>
        /// Adds a new element with the given name, corrected for uniqueness
        /// </summary>
        /// <param name="name"></param>
        public void NewElement(string name)
        {
            if (_elementsVMs.Any(x => x.Name == name))
            {
                int i = 0;
                while (_elementsVMs.Any(x => x.Name == $"{name} {i}"))
                {
                    i++;
                }
                name = $"{name} {i}";
            }

            var newElement = _factory.MakeMainElement(_type, _ele.InitialValue);

            _elementsVMs.Add(new ElementVM(name, newElement, this));
        }

        /// <summary>
        /// Removes a specific element
        /// </summary>
        /// <param name="elementVM"></param>
        public void RemoveElement(ElementVM elementVM) => _elementsVMs.Remove(elementVM);

        /// <summary>
        /// Removes all elements of this manager's type
        /// </summary>
        public void Clear()
        {
            foreach (var tab in _controller.Items)
            {
                if (tab is ElementEditor elementEditor
                    && _elementsVMs.Contains(elementEditor.ElementVM))
                {
                    _controller.Items.Remove(elementEditor);
                    return;
                }
            }

            _elementsVMs.Clear();
        }

        /// <summary>
        /// Adds a new elements of the manager's type
        /// </summary>
        public ICommand AddCommand => new Command((s) => NewElement($"new {_type.Name}"), (s) => true);

        /// <summary>
        /// Saves all elements to a folder at the given path
        /// </summary>
        /// <param name="path">Path to save to</param>
        public void Save(string path)
        {
            path = Path.Combine(path, ElementName);

            foreach (var elementVM in Elements)
            {
                //make a new object of this elements type, pass it around to fill in data, save it
                var instance = _constructor.Invoke(null);

                elementVM.Write(instance);

                // create the dir in case it doesn't exist
                Directory.CreateDirectory(path);

                var options = new JsonSerializerOptions();
                options.IncludeFields = _ele.IncludeFields;

                var jsonStr = JsonSerializer.Serialize(instance, options);

                File.WriteAllText(path + $"/{elementVM.Name}{Extension}", jsonStr);
            }
        }

        /// <summary>
        /// Loads data a specific eleemnts
        /// </summary>
        /// <param name="elementName">name of element to load into</param>
        /// <param name="value">json string value</param>
        public void Load(string elementName, string value)
        {
            var options = new JsonSerializerOptions();
            options.IncludeFields = _ele.IncludeFields;

            var instance = JsonSerializer.Deserialize(value, _type, options);
            var target = _elementsVMs.FirstOrDefault(x => x.Name == elementName);

            if (target != null && instance != null)
            {
                target.Element.Read(instance);
            }
        }
    }
}
