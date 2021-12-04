// UiSon, by Cameron Gale 2021

namespace UiSon.Attribute
{
    public abstract class UiSonAttribute : System.Attribute
    {
        public string RegionName { get; private set; }
        public int Priority { get; private set; }

        /// <summary>
        /// Base for a attribute. Used for identification, would be abstract but attribute classes cannot be abstract.
        /// </summary>
        /// <param name="groupName">The group this element belongs to</param>
        /// <param name="priority">The sorting priority for this eleemnt</param>
        internal UiSonAttribute(string groupName, int priority)
        {
            RegionName = groupName;
            Priority = priority;
        }
    }
}
