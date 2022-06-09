// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Selects from a drop down of strings
    /// </summary>
    public class SelectorModule : BaseEditorModule, ISelectorModule
    {
        /// <inheritdoc/>
        public virtual IEnumerable<string> Options => _view.Options;

        private readonly ISelectorValueView _view;

        public SelectorModule(ISelectorValueView view,
                              ModuleTemplateSelector templateSelector)
            :base(view, templateSelector)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ISelectorValueView.Options):
                    if (!Options.Contains(Value))
                    {
                        Value = "null";
                    }
                    OnPropertyChanged(nameof(Options));
                    break;
            }
        }
    }
}
