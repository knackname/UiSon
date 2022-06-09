// UiSon, by Cameron Gale 2022

using System.ComponentModel;
using System.Text.RegularExpressions;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view that validates input via a regex
    /// </summary>
    public class RegexValueView : NPCBase, IUiValueView
    {
        /// <inheritdoc/>
        public object? Value => _decorated.Value;

        /// <inheritdoc/>
        public object? DisplayValue => _decorated.DisplayValue;

        /// <inheritdoc/>
        public bool IsValueBad => Regex.IsMatch((_decorated.Value?.ToString() ?? string.Empty) ?? "null", _regexValidation) ? _decorated.IsValueBad : false;

        /// <inheritdoc/>
        public int DisplayPriority => _decorated.DisplayPriority;

        /// <inheritdoc/>
        public string? Name => _decorated.Name;

        /// <inheritdoc/>
        public UiType UiType => _decorated.UiType;

        /// <inheritdoc/>
        public Type? Type => _decorated.Type;

        private readonly IUiValueView _decorated;
        private readonly string _regexValidation;
        private readonly ValueMemberInfo? _info;

        public RegexValueView(IUiValueView decorated, string regexValidation, ValueMemberInfo? info)
        {
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));

            _decorated.PropertyChanged += OnDecoratedPropertyChanged;

            _regexValidation = regexValidation ?? throw new ArgumentNullException(nameof(regexValidation));

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
            }
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            var asString = (value as string) ?? "null";

            if (Regex.IsMatch(asString, _regexValidation))
            {
                return _decorated.TrySetValue(value);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TrySetValueFromRead(object? value)
        {
            var asString = (value as string) ?? "null";

            if (Regex.IsMatch(asString, _regexValidation))
            {
                return _decorated.TrySetValueFromRead(value);
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

            TrySetValue(_info.GetValue(instance));
        }

        /// <inheritdoc/>
        public void Write(object instance)
        {
            if (_info == null)
            {
                throw new Exception("Write called on view without member info");
            }

            _info.SetValue(instance, Value);
        }
    }
}
