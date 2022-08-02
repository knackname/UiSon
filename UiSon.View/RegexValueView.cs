// UiSon, by Cameron Gale 2022

using System.Text.RegularExpressions;
using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    /// <summary>
    /// A view that validates input via a regex
    /// </summary>
    public class RegexValueView : DecoratingView
    {
        /// <inheritdoc/>
        public override ModuleState State => _state ?? base.State;
        private ModuleState? _state;

        /// <inheritdoc/>
        public override string StateJustification => _stateJustification ?? base.StateJustification;
        private string? _stateJustification;

        private readonly string _regexValidation;

        public RegexValueView(IUiValueView decorated, ValueMemberInfo? info, string regexValidation)
            : base(decorated, info)
        {
            _regexValidation = regexValidation ?? throw new ArgumentNullException(nameof(regexValidation));
        }

        /// <inheritdoc/>
        public override void SetValue(object? value)
        {
            if (Regex.IsMatch((value as string) ?? "null", _regexValidation))
            {
                _state = null;
                _stateJustification = null;
            }
            else
            {
                _state = ModuleState.Error;
                _stateJustification = $"Value does not match regex: {_regexValidation}";
            }

            _decorated.SetValue(value);
        }

        /// <inheritdoc/>
        public override bool TrySetValue(object? value)
        {
            if (Regex.IsMatch((value as string) ?? "null", _regexValidation))
            {
                _state = null;
                _stateJustification = null;

                return base.TrySetValue(value);
            }

            return false;
        }

        /// <inheritdoc/>
        public override void SetValueFromRead(object? value)
        {
            if (Regex.IsMatch((value as string) ?? "null", _regexValidation))
            {
                _state = null;
                _stateJustification = null;
            }
            else
            {
                _state = ModuleState.Error;
                _stateJustification = $"Value does not match regex: {_regexValidation}";
            }

            _decorated.SetValueFromRead(value);
        }

        /// <inheritdoc/>
        public override bool TrySetValueFromRead(object? value)
        {
            if (Regex.IsMatch((value as string) ?? "null", _regexValidation))
            {
                _state = null;
                _stateJustification = null;

                return base.TrySetValueFromRead(value);
            }

            return false;
        }
    }
}
