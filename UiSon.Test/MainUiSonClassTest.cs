// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonElement]
    public class UiSonClassTest
    {
        [UiSonEncapsulatingUi]
        public List<Tuple<int,int>> Int_List;

        [UiSonEncapsulatingUi]
        public poop? Struct_Nullable;

        [UiSonEncapsulatingUi]
        public poop Struct;

        [UiSonEncapsulatingUi]
        public SingleSelector _singleSelector;

        [UiSonEncapsulatingUi]
        public Sliders _sliders;

        [UiSonEncapsulatingUi]
        public Attribulteless attribulteless;

        [UiSonEncapsulatingUi]
        public TextEdits _textEdits;

        [UiSonEncapsulatingUi]
        public Checkboxes _checkboxes;

        [UiSonEncapsulatingUi]
        public Selectors _selectors;

        [UiSonEncapsulatingUi]
        public ClassSelectors _classSelectors;

        [UiSonEncapsulatingUi]
        public Groups _groups;

        [UiSonEncapsulatingUi]
        public Collections _collections;

        [UiSonEncapsulatingUi]
        public SelfRef _selfRef;

        [UiSonEncapsulatingUi]
        public MultiChoiceUi multiChoice;

        [UiSonEncapsulatingUi]
        public TextBlocks _textBlocks;
    }

    public struct poop
    {
        public int a;

        [UiSonTextEditUi]
        public int b;

        public override string ToString() => $"a: {a}, b: {b}";
    }
}
