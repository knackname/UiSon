// UiSon, by Cameron Gale 2022

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
        private NotifyingCollection<ElementView> _elements = new NotifyingCollection<ElementView>();

        public Type ElementType => _type;
        private Type _type;

        private UiSonElementAttribute _elementAtt;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Element type</param>
        /// <param name="elementAtt">Element attribute</param>
        public ElementManager(Type type, UiSonElementAttribute elementAtt)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _elementAtt = elementAtt ?? throw new ArgumentNullException(nameof(elementAtt));
        }

        /// <summary>
        /// Adds a new element with the given name, corrected for uniqueness
        /// </summary>
        /// <param name="name"></param>
        public IElementView NewElement(string name, object? initialValue)
        {
            var newElement = new ElementView(name, initialValue, this);
            _elements.Add(newElement);
            OnPropertyChanged(nameof(Elements));
            return newElement;
        }

        /// <summary>
        /// Adds a new element with the given name, corrected for uniqueness
        /// </summary>
        public IElementView NewElement()
        {
            var newElement = new ElementView($"New {ElementName}", Activator.CreateInstance(_type), this);
            _elements.Add(newElement);
            OnPropertyChanged(nameof(Elements));
            return newElement;
        }

        /// <summary>
        /// Removes a specific element
        /// </summary>
        /// <param name="elementVM"></param>
        public void RemoveElement(ElementView elementVM)
        {
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
