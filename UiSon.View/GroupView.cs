// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class GroupView : NPCBase, IGroupView
    {
        /// <inheritdoc/>
        public IReadWriteView[] Members => _members;
        private readonly IReadWriteView[] _members;

        /// <inheritdoc/>
        public virtual bool IsValueBad => _members.Any(x => x.IsValueBad);

        /// <inheritdoc/>
        public int DisplayPriority { get; private set; }

        /// <inheritdoc/>
        public DisplayMode DisplayMode { get; private set; }

        /// <inheritdoc/>
        public string? Name { get; private set; }

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
                case nameof(IUiValueView.IsValueBad):
                    OnPropertyChanged(nameof(IsValueBad));
                    break;
                case nameof(IUiValueView.Value):
                    OnPropertyChanged(nameof(IUiValueView.Value));
                    break;
            }
        }

        /// <inheritdoc/>
        public virtual bool TrySetValue(object? value) => false;

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
