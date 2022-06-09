// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.View.Interface
{
    public interface IUiValueView : IReadWriteView
    {
        /// <summary>
        /// The view's ui type.
        /// </summary>
        UiType UiType { get; }

        /// <summary>
        /// The view's value.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// The view's display value.
        /// </summary>
        object? DisplayValue { get; }

        /// <summary>
        /// The view's value's type.
        /// </summary>
        Type? Type { get; }
    }
}
