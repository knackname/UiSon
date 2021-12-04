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

        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value)
                    && !_parent.Elements.Any(x => x.Name == value))
                {
                    var old = _name;
                    _name = value;

                    PropertyChanged.Invoke(this, new PropertyChangedExtendedEventArgs<string>(nameof(Name), old, _name));
                }
            }
        }
        private string _name;

        public IElement Element { get; private set; }

        private ElementManager _parent;

        public ElementVM(string name, IElement element, ElementManager parent)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            Element = element ?? throw new ArgumentNullException(nameof(element));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public void Write(object instance) => Element.Write(instance);

        public ICommand RemoveCommand => new Command((s) => _parent.RemoveElement(this), (s) => true);
    }
}
