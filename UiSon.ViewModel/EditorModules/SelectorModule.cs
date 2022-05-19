// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UiSon.Element;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// Selects from a drop down of strings
    /// </summary>
    public class SelectorModule : BaseEditorModule, ISelectorModule
    {
        public override object Value
        {
            get => base.Value ?? "null";
            set
            {
                if (_view.Value != value)
                {
                    base.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public override ModuleState State => _view.Value == null ? ModuleState.Special : base.State;

        public virtual IEnumerable<string> Options => _view.Options;

        private readonly new ISelectorView _view;

        public SelectorModule(ISelectorView view,
                              ModuleTemplateSelector templateSelector,
                              string name,
                              int displayPriority)
            :base(view, templateSelector, name, displayPriority)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            _view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IReadWriteView.Value):
                    //opc value handled by base
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }
    }
}
