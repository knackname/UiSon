// UiSon, by Cameron Gale 2022

using System;
using System.ComponentModel;
using System.Linq;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;
using UiSon.ViewModel.Interface;

namespace UiSon.ViewModel
{
    public class MultiChoiceModule : GroupModule, IValueEditorModule
    {
        /// <inheritdoc/>
        public object Value
        {
            get => _view.DisplayValue;
            set => _view.TrySetValue(value);
        }

        //public object Value
        //{
        //    get
        //    {
        //        var value = Activator.CreateInstance(_collectionType);

        //        // some implimentations of ICollection<> may have rules about what can be added and throw an error
        //        // in that case refresh the old value back into the child moduels
        //        try
        //        {
        //            foreach (var member in Members)
        //            {
        //                if (((bool?)member.Value) ?? false)
        //                {
        //                    _collectionAdd.Invoke(value, new[] { member.Name.ParseAs(_entryType) });
        //                }
        //            }

        //            _isValueBad = false;
        //            return value;
        //        }
        //        catch
        //        {
        //            _isValueBad = true;
        //            return null;
        //        }
        //    }
        //    set
        //    {
        //        if (value is IEnumerable enumerable
        //            && value.GetType()
        //                    .GetInterfaces()
        //                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>))
        //                    .GetGenericArguments()
        //                    .FirstOrDefault() == _entryType)
        //        {
        //            foreach (var member in Members)
        //            {
        //                member.Value = false;
        //            }

        //            foreach (var entry in enumerable)
        //            {
        //                var member = Members.FirstOrDefault(x => x.Name == entry.ToString());

        //                if (member != null)
        //                {
        //                    member.Value = true;
        //                }
        //            }

        //            _isValueBad = true;
        //            OnPropertyChanged(nameof(Members));
        //            OnPropertyChanged(nameof(Value));
        //            OnPropertyChanged(nameof(State));
        //        }
        //    }
        //}
        //private bool _isValueBad;

        /// <inheritdoc/>
        public override ModuleState State => _view.IsValueBad ? ModuleState.Error : base.State;

        public IUiValueView View => null;

        private readonly IValueEditorModule[] _members;
        private readonly ICollectionValueView _view;

        public MultiChoiceModule(ICollectionValueView view,
                                 string name,
                                 int priority,
                                 DisplayMode displayMode,
                                 IValueEditorModule[] members)
            :base(name, priority, displayMode, members)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _view.PropertyChanged += OnViewPropertyChanged;

            foreach(var member in members)
            {
                member.PropertyChanged += OnMemberPropertyChanged;
            }
        }

        private void OnViewPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ICollectionValueView.DisplayValue):
                    RepopulateMembers();
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(ICollectionValueView.DisplayMode):
                    OnPropertyChanged(nameof(DisplayMode));
                    break;
                case nameof(ICollectionValueView.DisplayPriority):
                    OnPropertyChanged(nameof(DisplayPriority));
                    break;
                case nameof(ICollectionValueView.IsValueBad):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        private void OnMemberPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IValueEditorModule.Value):

                    OnPropertyChanged(nameof(Value));
                    break;
            }
        }

        private void RepopulateMembers()
        {
            foreach (var member in _members)
            {
                member.Value = false;
            }

            foreach (var entry in _view.Entries)
            {
                var member = _members.FirstOrDefault(x => x.Name == entry.ToString());

                if (member != null)
                {
                    member.Value = true;
                }
            }
        }
    }
}
