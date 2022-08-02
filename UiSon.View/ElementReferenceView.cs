// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class ElementReferenceView : NPCBase
    {
        /// <summary>
        /// The identifying view's value, or the element's name if there isn't one.
        /// </summary>
        public object? Value => _identifingView == null
            ? _element?.Name
            : _identifingView.Value;

        /// <summary>
        /// The name of the referenced element.
        /// </summary>
        public object? DisplayValue => _element?.Name;

        /// <summary>
        /// If the reference is set
        /// </summary>
        public bool HasReference => _element != null;

        /// <summary>
        /// The manager of the referenced element.
        /// </summary>
        public IEnumerable<string> ElementOptions => _elementManager?.Elements.Select(x => x.Name) ?? Enumerable.Empty<string>();

        private IElementManager? _elementManager;
        private IElementView? _element;
        private IUiValueView? _identifingView;

        private readonly string? _identifingTagName;
        private readonly string _elementName;
        private readonly IHaveProject _hasProject;

        public ElementReferenceView(IHaveProject hasProject, string elementName, string? identifingTag)
        {
            _hasProject = hasProject ?? throw new ArgumentNullException(nameof(hasProject));
            _hasProject.ProjectChanged += OnProjectChanged;
            _elementName = elementName;
            _identifingTagName = identifingTag;

            UpdateReference();
        }

        private void OnProjectChanged(object? sender, PropertyChangedEventArgs e) => UpdateReference();

        private void OnElementManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ElementManager.Elements):
                    OnPropertyChanged(nameof(ElementOptions));
                    break;
            }
        }

        private void OnElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IElementView.Name):
                    OnPropertyChanged(nameof(DisplayValue));
                    break;
            }
        }

        private bool TrySetElement(IElementView? element)
        {
            if (element != null)
            {
                if (_element != null)
                {
                    _element.PropertyChanged -= OnElementPropertyChanged;
                }

                _element = element;
                _element.PropertyChanged += OnElementPropertyChanged;

                _identifingView = null;

                if (_identifingTagName != null)
                {
                    if (!_element.TagNameToView.ContainsKey(_identifingTagName))
                    {
                        return false;
                    }

                    // when grabbing the element we've already confirmed it has the tag
                    _identifingView = _element.TagNameToView[_identifingTagName];
                }

                OnPropertyChanged(nameof(Value));
                OnPropertyChanged(nameof(HasReference));
                OnPropertyChanged(nameof(DisplayValue));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to set the reference to the element with the given name
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TrySetElementFromName(string? name) => TrySetElement(_elementManager?.Elements.FirstOrDefault(x => x.Name == name));

        /// <summary>
        /// Attempts to set the reference to the element with the given name
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool TrySetElementFromValue(object? value) => _identifingTagName == null
            ? TrySetElementFromName(value as string)
            : TrySetElement(_elementManager?.Elements
                                            .FirstOrDefault(x => x.TagNameToView.ContainsKey(_identifingTagName)
                                                            && x.TagNameToView[_identifingTagName].Value == value));

        /// <summary>
        /// Removes the reference.
        /// </summary>
        public void ClearReference()
        {
            if (_element != null)
            {
                _element.PropertyChanged -= OnElementPropertyChanged;
            }
            
            _element = null;
            _identifingView = null;

            OnPropertyChanged(nameof(HasReference));
            OnPropertyChanged(nameof(Value));
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateReference()
        {
            if (_elementManager != null)
            {
                _elementManager.PropertyChanged -= OnElementManagerPropertyChanged;
            }

            _elementManager = _hasProject.Project.ElementManagers.FirstOrDefault(x => x.ElementName == _elementName);

            if (_elementManager != null)
            {
                _elementManager.PropertyChanged += OnElementManagerPropertyChanged;
            }

            TrySetElementFromValue(Value);

            OnPropertyChanged(nameof(ElementOptions));
        }
    }
}
