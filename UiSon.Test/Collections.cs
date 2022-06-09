// by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    public class Collections
    {
        [UiSonCollection(true, false, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<string> stack;

        [UiSonCollection(true, false, DisplayMode.Grid)]
        [UiSonTextEditUi]
        public List<string> grid;

        [UiSonCollection(true, false, DisplayMode.Grid)]
        [UiSonTextEditUi]
        public List<List<string>> nestedGrid;

        [UiSonCollection(true, false, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<string> collection;

        [UiSonCollection(true, false, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<List<string>> nestedCollection;

        [UiSonCollection]
        [UiSonTextEditUi]
        public List<List<string>> inited_nestedStrangList = new List<List<string>>() { new List<string>() { "A", "B", "C" }, new List<string>() { "1", "2", "3" } };

        [UiSonCollection]
        [UiSonTextEditUi]
        public int int_with_enumerable_att;

        [UiSonCollection]
        [UiSonTextEditUi]
        public string string_with_enumerable_att;

        [UiSonCollection]
        [UiSonTextEditUi]
        public List<ParameteredClass> parametered_members;

        [UiSonCollection]
        [UiSonMemberElementUi]
        public List<SelfRef> selfRefing_members;

        [UiSonCollection(true, false, DisplayMode.Grid)]
        [UiSonMemberElementUi]
        public List<SelfRef> selfRefing_members_grid;

        [UiSonCollection(true, false, DisplayMode.Vertial)]
        [UiSonMemberElementUi]
        public Dictionary<string, string> dict_stack;

        [UiSonCollection(true, false, DisplayMode.Grid)]
        [UiSonMemberElementUi]
        public Dictionary<string, string> dict_grid;

        [UiSonCollection(true, false, DisplayMode.Vertial)]
        [UiSonMemberElementUi]
        public List<UnparameteredClass> unparam_stack;

        [UiSonCollection(true, false, DisplayMode.Grid)]
        [UiSonMemberElementUi]
        public List<UnparameteredClass> unparam_grid;

        [UiSonCollection(false, false, DisplayMode.Grid)]
        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass))]
        public List<string> eleSelector_grid = new List<string> {"null",null,"null"};

        [UiSonCollection(false, false, DisplayMode.Vertial)]
        [UiSonElementSelectorUi(nameof(ReferencedUiSonClass))]
        public List<string> eleSelector_stack = new List<string> { "null", null, "null" };
    }

    public class ParameteredClass
    {
        public string Name {get; private set;}
        public ParameteredClass(string name)
        {
            Name = name;
        }
    }

    public class UnparameteredClass
    {
        public string A { get; private set; }
        public string B { get; private set; }
        public string C { get; private set; }
    }
}
