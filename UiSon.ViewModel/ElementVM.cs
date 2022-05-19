// UiSon, by Cameron Gale 2021

using System;
using System.Windows.Input;
using UiSon.Command;
using UiSon.View;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A user creatable element
    /// </summary>
    public class ElementVM : NPCBase, IElementVM
    {
        /// <summary>
        /// Unique name for the element
        /// </summary>
        public string Name
        {
            get => _view.Name;
            set => _view.Name = value;
        }

        private IDisplayElements _parent;
        private ElementView _view;

        /// <summary>
        /// Constructor
        /// </summary>
        public ElementVM(ElementView view,
                         IDisplayElements parent)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        #region Commands

        public ICommand CloseCommand => new UiSonActionCommand((s) => _parent.CloseElement(this));

        #endregion
    }
}
