// UiSon, by Cameron Gale 2022

using UiSon.Attribute;

namespace UiSon.Test
{
    [UiSonGroup("Vertical", 0, null, DisplayMode.Vertial)]
    [UiSonGroup("Horizontal", 0, null, DisplayMode.Horizontal)]
    [UiSonGroup("Wrap", 0, null, DisplayMode.Wrap)]
    [UiSonGroup("Grid", 0, null, DisplayMode.Grid)]
    [UiSonGroup("Parent", 0, null)]
    [UiSonGroup("Child", 0, "Parent")]
    public class Groups
    {
        // undefined members

        [UiSonCheckboxUi]
        public bool NoGroup_member;

        [UiSonCheckboxUi(0, null)]
        public bool NullGroup_member;

        [UiSonCheckboxUi(0, "Undefined")]
        public bool UndefinedGroup_member;

        // Defined members

        [UiSonCheckboxUi(0, "Vertical")]
        public bool V_member_A;

        [UiSonCheckboxUi(0, "Vertical")]
        public bool V_member_B;

        [UiSonCheckboxUi(0, "Horizontal")]
        public bool H_member_A;

        [UiSonCheckboxUi(0, "Horizontal")]
        public bool H_member_B;

        [UiSonCheckboxUi(0, "Wrap")]
        public bool W_member_A;

        [UiSonCheckboxUi(0, "Wrap")]
        public bool W_member_B;

        [UiSonCheckboxUi(0, "Grid")]
        public bool G_member_A;

        [UiSonCheckboxUi(0, "Grid")]
        public bool G_member_B;

        [UiSonCheckboxUi(0, "Child")]
        public bool C_member_A;
    }
}
