// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// Contains all of one type of UiSonElement
    /// </summary>
    public class ElementManager : NPCBase, IElementManager
    {
        /// <summary>
        /// Name of the element
        /// </summary>
        public string ElementName => _type.Name;

        /// <summary>
        /// Extension type of the elements file
        /// </summary>
        public string Extension => _elementAtt.Extension;

        /// <summary>
        /// The elements of this manager's type
        /// </summary>
        public IEnumerable<IElementView> Elements => _elements.OrderBy(x => x.Name);
        private NotifyingCollection<IElementView> _elements = new NotifyingCollection<IElementView>();

        public Type ElementType => _type;
        private Type _type;

        private readonly UiSonElementAttribute _elementAtt;
        private readonly ViewFactory _factory;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Element type</param>
        /// <param name="elementAtt">Element attribute</param>
        public ElementManager(Type type, UiSonElementAttribute elementAtt, ViewFactory factory)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _elementAtt = elementAtt ?? throw new ArgumentNullException(nameof(elementAtt));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IElementView.Value):
                    OnPropertyChanged("Element");
                    break;
            }
        }

        /// <summary>
        /// Adds a new element with the given name, corrected for uniqueness
        /// </summary>
        /// <param name="name"></param>
        public IElementView NewElement(string name)
        {
            var newElement = _factory.MakeElementView(name, _type, _elementAtt.AutoGenerateMemberAttributes, this);
            newElement.PropertyChanged += OnElementPropertyChanged;
            _elements.Add(newElement);
            OnPropertyChanged(nameof(Elements));
            return newElement;
        }

        /// <summary>
        /// Adds a new element with default name and value
        /// </summary>
        /// <returns></returns>
        public IElementView NewDefaultElement()
        {
            var newElement = NewElement($"New {ElementName}");
            newElement.MainView.SetValue(Activator.CreateInstance(_type));
            return newElement;
        }

        /// <summary>
        /// Removes a specific element
        /// </summary>
        /// <param name="elementVM"></param>
        public void RemoveElement(IElementView elementVM)
        {
            elementVM.PropertyChanged -= OnElementPropertyChanged;
            _elements.Remove(elementVM);
            OnPropertyChanged(nameof(Elements));
        }

        /// <summary>
        /// Saves all elements to a folder at the given path 
        /// </summary>
        /// <param name="path">Path to save to</param>
        public void Save(string path)
        {
            path = Path.Combine(path, ElementName);
            Directory.CreateDirectory(path);

            foreach (var element in Elements)
            {
                element.Save(path);
            }
        }
    }
}
