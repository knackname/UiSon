// UiSon, by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    public class UiSonClassTest
    {
        [UiSonMemberElementUi]
        public SingleSelector _singleSelector;

        [UiSonMemberElementUi]
        public Sliders _sliders;

        [UiSonMemberElementUi]
        public Attribulteless attribulteless;

        [UiSonMemberElementUi]
        public TextEdits _textEdits;

        [UiSonMemberElementUi]
        public Checkboxes _checkboxes;

        [UiSonMemberElementUi]
        public Selectors _selectors;

        [UiSonMemberElementUi]
        public ClassSelectors _classSelectors;

        [UiSonMemberElementUi]
        public Groups _groups;

        [UiSonMemberElementUi]
        public Collections _collections;

        [UiSonMemberElementUi]
        public SelfRef _selfRef;

        [UiSonMemberElementUi]
        public MultiChoiceUi multiChoice;

        [UiSonMemberElementUi]
        public TextBlocks _textBlocks;
    }
}
