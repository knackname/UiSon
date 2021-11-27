// UiSon, by Cameron Gale 2021

namespace UiSon.Element
{
    public abstract class ValElement<T> : BaseElement
    {
        public abstract T Value { get; set; }

        public ValElement(string name, int priority)
            :base(name, priority)
        {
        }
    }
}
