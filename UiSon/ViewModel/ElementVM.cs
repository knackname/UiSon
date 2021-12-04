// UiSon, by Cameron Gale 2021

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using UiSon.Commands;
using UiSon.Element.Element.Interface;
using UiSon.Events;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A user creatable element
    /// </summary>
    public class ElementVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Unique name for the element
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                //names must not be whitespace
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // make sure name is unique
                    while (_parent.Elements.Any(x => x.Name == value))
                    {
                        value += " - Copy";
                    }

                    var old = _name;
                    _name = value;

                    PropertyChanged.Invoke(this, new PropertyChangedExtendedEventArgs<string>(nameof(Name), old, _name));
                }
            }
        }
        private string _name;

        public IElement Element { get; private set; }

        private ElementManager _parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The element's name</param>
        /// <param name="element">The internal element</param>
        /// <param name="parent">The vm's parent collection, so it can remove itself</param>
        public ElementVM(string name, IElement element, ElementManager parent)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            Element = element ?? throw new ArgumentNullException(nameof(element));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Writes the element's data to an instance of the class it represents
        /// </summary>
        /// <param name="instance"></param>
        public void Write(object instance) => Element.Write(instance);

        /// <summary>
        /// Removes this vm from its parent collection
        /// </summary>
        public ICommand RemoveCommand => new Command((s) => _parent.RemoveElement(this), (s) => true);
    }
}
