// UiSon, by Cameron Gale 2021

using System;
using UiSon.Attribute.Enums;

namespace UiSon.Attribute
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class UiSonCollectionAttribute : UiSonAttribute
    {
        public bool Modifiable { get; private set; }
        public CollectionType CollectionType { get; private set; }
        public DisplayMode DisplayMode { get; private set; }
        public Alignment Alignment { get; private set; }
        public Type EntryType { get; private set; }

        public UiSonCollectionAttribute(Type entryType, string name = null, int priority = 0, string regionName = null,
                                        bool modifiable = true, DisplayMode displayMode = DisplayMode.Vertial, Alignment alignment = Alignment.Left, CollectionType collectionType = CollectionType.Template)
            :base(name, regionName, priority)
        {
            Modifiable = modifiable;
            CollectionType = collectionType;
            DisplayMode = displayMode;
            EntryType = entryType ?? throw new ArgumentNullException(nameof(entryType));
            Alignment = alignment;
        }
    }
}
