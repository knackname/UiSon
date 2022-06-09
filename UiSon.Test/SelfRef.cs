﻿// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    public class SelfRef
    {
        [UiSonTextEditUi]
        public int otherMember;

        [UiSonMemberElementUi(0, null, DisplayMode.Grid)]
        public SelfRef _selfRef;
    }
}
