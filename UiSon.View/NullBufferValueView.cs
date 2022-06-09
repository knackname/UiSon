// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view that isn't created until its value is first requested, prevents infinate loops on views of types that reference itself
    /// </summary>
    public class NullBufferValueView : NPCBase, IUiValueView
    {
        /// <summary>
        /// If the value is null.
        /// </summary>
        public bool IsNull
        {
            get => _isNull;
            set
            {
                if (!value && _decorated == null)
                {
                    _decorated = _makeDecorated.Invoke();
                    _decorated.PropertyChanged += OnDecoratedPropertyChanged;

                    // make null so it can be cleaned up, only used here this once
                    _makeDecorated = null;

                    //OnPropertyChanged();
                    //OnPropertyChanged(nameof(Value));
                    //OnPropertyChanged(nameof(DisplayValue));
                    //OnPropertyChanged(nameof(IsValueBad));
                }

                if (_isNull != value)
                {
                    _isNull = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        private bool _isNull = true;

        /// <inheritdoc/>
        public int DisplayPriority => _decorated?.DisplayPriority ?? _displayPriority;
        private readonly int _displayPriority;

        /// <inheritdoc/>
        public string Name => _decorated?.Name ?? _name;
        private readonly string _name;

        /// <inheritdoc/>
        public Type? Type => _decorated?.Type;

        /// <inheritdoc/>
        public object? Value => _isNull ? null : Decorated.Value;

        /// <inheritdoc/>
        public object? DisplayValue => _isNull ? null : Decorated.DisplayValue;

        /// <inheritdoc/>
        public bool IsValueBad => _isNull ? false : Decorated.IsValueBad;

        /// <inheritdoc/>
        public UiType UiType => Decorated.UiType;

        /// <summary>
        /// The buffer's decorated view
        /// </summary>
        public IUiValueView Decorated => _decorated == null ? throw new Exception("Decorated dependant property accessed before becoming non-null") : _decorated;
        private IUiValueView? _decorated;
        private Func<IUiValueView> _makeDecorated;

        private readonly ValueMemberInfo? _info;

        public NullBufferValueView(int displayPriority, string name, ValueMemberInfo? info, Func<IUiValueView> makeDecorated)
        {
            _makeDecorated = makeDecorated ?? throw new ArgumentNullException(nameof(makeDecorated));

            _displayPriority = displayPriority;
            _name = name;
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
                case nameof(IUiValueView.IsValueBad):
                    OnPropertyChanged(nameof(IsValueBad));
                    break;
                case nameof(IUiValueView.UiType):
                    OnPropertyChanged(nameof(UiType));
                    break;
            }
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            if (value == null)
            {
                IsNull = true;
                return true;
            }
            else
            {
                IsNull = false;

                if (Decorated.TrySetValue(value))
                {
                    IsNull = false;
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            if (value == null)
            {
                IsNull = true;
                OnPropertyChanged(nameof(IsNull));
                return true;
            }
            else
            {
                IsNull = false;

                if (Decorated.TrySetValueFromRead(value))
                {
                    OnPropertyChanged(nameof(IsNull));
                    return true;
                }
            }

            return false;
        }

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

            if (!IsValueBad)
            {
                _info.SetValue(instance, Value);
            }
        }
    }
}
