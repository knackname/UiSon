// UiSon, by Cameron Gale 2022

using UiSon.Element;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class SelectorView : NPCBase, ISelectorView
    {
        /// <inheritdoc/>
        public IEnumerable<string> Options => _converter.FirstValues;

        /// <inheritdoc/>
        public object? Value => _decorated.Value;

        private readonly StringView _decorated;
        private readonly Map<string, string> _converter;

        private bool _badValue = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="decorated"></param>
        /// <param name="converter"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectorView(StringView decorated, Map<string, string> converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        }

        /// <inheritdoc/>
        public bool TrySetValue(object? value)
        {
            var strValue = value?.ToString();

            if (strValue == null || strValue == "null")
            {
                return _decorated.TrySetValue(null);
            }
            else if (_decorated.TrySetValue(_converter.FirstToSecond[strValue]))
            {
                // having been set from the converter's options, the value can no longer be bad
                _badValue = false;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual bool TrySetValueFromRead(object? value)
        {
            var strValue = value?.ToString();

            if (strValue == null || strValue == "null")
            {
                return _decorated.TrySetValue(null);
            }
            else if (_converter.SecondValues.Contains(strValue))
            {
                return _decorated.TrySetValue(value);
            }

            return false;
        }

        /// <inheritdoc/>
        public void Read(object instance)
        {
            _decorated.Read(instance);

            // The _element could have read in a value not present in the converter. Set _badValue in that case so a bad value isn't looked up
            var readVal = (string)_decorated.Value;

            if (!_converter.SecondValues.Any(x => x == readVal))
            {
                // a bad value was read, oh no!
                // try setting it to null
                if (_decorated.TrySetValue(null))
                {
                    _badValue = false;
                }
                else
                {
                    // otherwise lets try to pick a value
                    foreach (var value in _converter.SecondValues)
                    {
                        if (_decorated.TrySetValue(value))
                        {
                            _badValue = false;
                            return;
                        }
                    }

                    // mission failed, we'll get 'em next time
                    _badValue = true;
                }
            }
        }

        /// <inheritdoc/>
        public void Write(object instance) => _decorated.Write(instance);
    }
}
