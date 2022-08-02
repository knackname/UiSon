// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UiSon.Notify.Interface;
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

        /// <inheritdoc/>
        public override Type ValueType => typeof(string);

        private readonly ISelectorValueView _view;

        public SelectorModule(ISelectorValueView view,
                              ModuleTemplateSelector templateSelector,
                              ClipBoardManager clipBoardManager,
                              INotifier notifier)
            :base(view, templateSelector, clipBoardManager, notifier)
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
                        Value = Options.FirstOrDefault();
                    }
                    OnPropertyChanged(nameof(Options));
                    break;
            }
        }
    }
}
