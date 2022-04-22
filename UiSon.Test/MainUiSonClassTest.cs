// UiSon, by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    public class UiSonClassTest
    {
        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public Sliders _sliders;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public Attribulteless attribulteless;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public TextEdits _textEdits;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public Checkboxes _checkboxes;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public Selectors _selectors;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public EnumSelectors _enumSelectors;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public ClassSelectors _classSelectors;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public Groups _groups;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public Collections _collections;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public SelfRef _selfRef;

        [UiSonMemberElement(0, null, DisplayMode.Vertial)]
        public MultiChoiceUi multiChoice;
    }
}
