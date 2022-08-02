using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    public abstract class DecoratingView : NPCBase, IUiValueView
    {
        /// <inheritdoc/>
        public UiType UiType => _decorated.UiType;

        /// <inheritdoc/>
        public virtual object? Value => _decorated.Value;

        /// <inheritdoc/>
        public virtual object? DisplayValue => _decorated.DisplayValue;

        /// <inheritdoc/>
        public virtual Type? Type => _decorated.Type;

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public string? Name => _decorated.Name;

        /// <inheritdoc/>
        public virtual ModuleState State => _decorated.State;

        /// <inheritdoc/>
        public virtual string StateJustification => String.Empty;

        protected readonly IUiValueView _decorated;
        private readonly ValueMemberInfo? _info;

        public DecoratingView(IUiValueView decorated, ValueMemberInfo? info)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
            _decorated.PropertyChanged += OnDecoratedPropertyChanged;
            _info = info;
        }

        private void OnDecoratedPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IUiValueView.Value):
                    OnPropertyChanged(nameof(Value));
                    break;
                case nameof(IUiValueView.DisplayValue):
                    OnPropertyChanged(nameof(DisplayValue));
                    break;
                case nameof(IUiValueView.State):
                    OnPropertyChanged(nameof(State));
                    break;
            }
        }

        /// <inheritdoc/>
        public virtual void SetValue(object? value) => _decorated.SetValue(value);

        /// <inheritdoc/>
        public virtual bool TrySetValue(object? value) => _decorated.TrySetValue(value);

        /// <inheritdoc/>
        public virtual void SetValueFromRead(object? value) => _decorated.SetValueFromRead(value);

        /// <inheritdoc/>
        public virtual bool TrySetValueFromRead(object? value) => _decorated.TrySetValueFromRead(value);

        /// <inheritdoc/>
        public void Read(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Read called on view without member info");
            }

            TrySetValueFromRead(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Write called on view without member info");
            }

            if (State != ModuleState.Error)
            {
                _info.SetValue(instance, Value);
            }
        }
    }
}
