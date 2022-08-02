// UiSon, by Cameron Gale 2022

using System.Collections;
using UiSon.Attribute;
using UiSon.Element;
using UiSon.Extension;
using UiSon.View.Interface;

namespace UiSon.View
{
    public class ArrayValueView : CollectionValueView
    {
        /// <inheritdoc/>
        public override ModuleState State => _state ?? base.State;
        private ModuleState? _state;

        /// <inheritdoc/>
        public override string StateJustification => _stateJustification ?? base.StateJustification;
        private string? _stateJustification;

        /// <inheritdoc/>
        public override object? Value
        {
            get
            {
                var value = Array.CreateInstance(_entryType, _entries.Count) as IList;

                int index = 0;
                foreach (var entry in _entries)
                {
                    if (entry.Value.TryCast(_entryType, out object cast))
                    {
                        value[index] = cast;
                    }
                    index++;
                }

                return value;
            }
        }

        public ArrayValueView(ViewFactory factory,
                              Type type,
                              UiSonUiAttribute entryAttribute,
                              bool canModify,
                              int displayPriority,
                              string name,
                              DisplayMode displayMode,
                              ValueMemberInfo? info)
            : base(factory,
                  type,
                  false,// arrays wont have any
                  type.GetElementType(),
                  entryAttribute,
                  canModify,
                  displayPriority,
                  name,
                  displayMode,
                  info,
                  Array.Empty<IReadWriteView>())
        {
        }
    }
}
