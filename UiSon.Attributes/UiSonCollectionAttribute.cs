// UiSon, by Cameron Gale 2021

using System;

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

        /// <summary>
        /// Can only be used on collections. Requires an additional attribute to describe the Ui used by the collection members.
        /// </summary>
        /// <param name="entryType">Type used in the collection</param>
        /// <param name="priority"></param>
        /// <param name="groupName"></param>
        /// <param name="modifiable">Whether or not users can add or remove members from the collection</param>
        /// <param name="displayMode">The way in which members are displayed</param>
        /// <param name="alignment">The alignment of members</param>
        /// <param name="collectionType">The type of collection Ui</param>
        public UiSonCollectionAttribute(Type entryType, int priority = 0, string groupName = null,
                                        bool modifiable = true, DisplayMode displayMode = DisplayMode.Vertial, Alignment alignment = Alignment.Left, CollectionType collectionType = CollectionType.Template)
            :base(groupName, priority)
        {
            Modifiable = modifiable;
            CollectionType = collectionType;
            DisplayMode = displayMode;
            EntryType = entryType ?? throw new ArgumentNullException(nameof(entryType));
            Alignment = alignment;
        }
    }
}
