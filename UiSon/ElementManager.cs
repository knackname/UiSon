// UiSon, by Cameron Gale 2021

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ElementManager
    {
        public string ElementName => _ele.Name;
        public string Extension => _ele.Extension;

        public ObservableCollection<ElementVM> Elements => _elementsVMs;
        private ObservableCollection<ElementVM> _elementsVMs = new ObservableCollection<ElementVM>();

        private UiSonElementAttribute _ele;
        private TabControl _controller;
        private IEnumerable<ElementManager> _parent;
        private Type _type;
        private ElementFactory _factory;
        private ConstructorInfo _constructor;

        public ElementManager(Type type, ConstructorInfo constructor, TabControl controller, IEnumerable<ElementManager> parent, ElementFactory factory)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
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

        public void RemoveElement(ElementVM elementVM) => _elementsVMs.Remove(elementVM);

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

        public ICommand AddCommand => new Command((s) => NewElement($"new {_ele.Name}"), (s) => true);

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

                var jsonStr = JsonSerializer.Serialize(instance);

                File.WriteAllText(path + $"/{elementVM.Name}{Extension}", jsonStr);
            }
        }

        public void Load(string elementName, string value)
        {
            var instance = JsonSerializer.Deserialize(value, _type);
            var target = _elementsVMs.FirstOrDefault(x => x.Name == elementName);

            if (target != null && instance != null)
            {
                target.Element.Read(instance);
            }
        }
    }
}
