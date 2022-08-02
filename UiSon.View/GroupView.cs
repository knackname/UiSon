// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class GroupView : NPCBase, IGroupView
    {
        /// <inheritdoc/>
        public IReadWriteView[] Members => _members;
        private readonly IReadWriteView[] _members;

        /// <inheritdoc/>
        public int DisplayPriority { get; private set; }

        /// <inheritdoc/>
        public DisplayMode DisplayMode { get; private set; }

        /// <inheritdoc/>
        public string? Name { get; private set; }

        /// <inheritdoc/>
        public virtual ModuleState State => _state;
        private ModuleState _state;

        /// <inheritdoc/>
        public virtual string StateJustification => _stateJustification;
        private string _stateJustification = string.Empty;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="displayPriority"></param>
        /// <param name="name"></param>
        /// <param name="displayMode"></param>
        /// <param name="members"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GroupView(int displayPriority, string? name, DisplayMode displayMode, IReadWriteView[] members)
        {
            _members = members ?? throw new ArgumentNullException(nameof(members));

            foreach (var member in members)
            {
                member.PropertyChanged += OnMemberPropertyChanged;
            }

            DisplayPriority = displayPriority;
            Name = name;
            DisplayMode = displayMode;
        }

        private void OnMemberPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.State):
                    _state = _members.Any(x => x.State == ModuleState.Error) ? ModuleState.Error : ModuleState.Normal;
                    OnPropertyChanged(nameof(State));
                    break;
                case nameof(IUiValueView.Value):
                    OnPropertyChanged(nameof(IUiValueView.Value));
                    break;
            }
        }

        /// <inheritdoc/>
        public virtual void SetValue(object? value)
        {
            // no op
        }

        /// <inheritdoc/>
        public virtual bool TrySetValue(object? value) => false;

        /// <inheritdoc/>
        public void SetValueFromRead(object? value) => SetValue(value);

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value) => TrySetValue(value);

        /// <inheritdoc/>
        public virtual void Read(object instance)
        {
            foreach (var member in _members)
            {
                member.Read(instance);
            }
        }

        /// <inheritdoc/>
        public virtual void Write(object instance)
        {
            foreach (var member in _members)
            {
                member.Write(instance);
            }
        }
    }
}
