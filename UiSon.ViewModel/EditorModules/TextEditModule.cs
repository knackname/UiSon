// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Element;
using UiSon.View;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    /// <summary>
    /// A text editor for string values
    /// </summary>
    public class TextEditModule : BaseEditorModule, ITextEditModule
    {
        public override object Value
        {
            get => base.Value ?? "null";
            set
            {
                if (base.Value != value)
                {
                    base.Value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public override ModuleState State => base.Value == null ? ModuleState.Special : base.State;

        /// <summary>
        /// Constructor
        /// </summary>
        public TextEditModule(IReadWriteView view,
                                 ModuleTemplateSelector templateSelector,
                                 string name,
                                 int priority)
            :base(view, templateSelector, name, priority)
        {
            view.PropertyChanged += OnViewPropertyChanged;
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IReadWriteView.Value):
                    // opc value already handled by base
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }
    }
}
