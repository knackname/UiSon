// UiSon, by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    public class UiSonClassTest
    {
        [UiSonMemberElement]
        public Sliders _sliders;

        [UiSonMemberElement]
        public Attribulteless attribulteless;

        [UiSonMemberElement]
        public TextEdits _textEdits;

        [UiSonMemberElement]
        public Checkboxes _checkboxes;

        [UiSonMemberElement]
        public Selectors _selectors;

        [UiSonMemberElement]
        public ClassSelectors _classSelectors;

        [UiSonMemberElement]
        public Groups _groups;

        [UiSonMemberElement]
        public Collections _collections;

        [UiSonMemberElement]
        public SelfRef _selfRef;

        [UiSonMemberElement]
        public MultiChoiceUi multiChoice; 
        
        [UiSonMemberElement]
        public TextBlocks _textBlocks;
    }
}
