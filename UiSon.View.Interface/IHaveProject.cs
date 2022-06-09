// UiSon, by Cameron Gale 2022

using System.ComponentModel;

namespace UiSon.View.Interface
{
    public interface IHaveProject
    {
        event PropertyChangedEventHandler? ProjectChanged;
        IUiSonProject Project { get; }
    }
}
