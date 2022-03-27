// by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    public class Collections
    {
        [UiSonCollection(true, CollectionStyle.Stack, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<string> stack;

        [UiSonCollection(true, CollectionStyle.Grid, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<string> grid;

        [UiSonCollection(true, CollectionStyle.Grid, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<List<string>> nestedGrid;

        [UiSonCollection(true, CollectionStyle.Stack, DisplayMode.Vertial)]
        [UiSonTextEditUi]
        public List<string> collection;

        [UiSonCollection(true, CollectionStyle.Stack, DisplayMode.Vertial)]
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
        [UiSonMemberElement]
        public List<SelfRef> selfRefing_members;

        [UiSonCollection(true, CollectionStyle.Grid, DisplayMode.Vertial)]
        [UiSonMemberElement]
        public List<SelfRef> selfRefing_members_grid;

        [UiSonCollection(true, CollectionStyle.Stack, DisplayMode.Vertial)]
        [UiSonMemberElement]
        public Dictionary<string, string> dict_stack;

        [UiSonCollection(true, CollectionStyle.Grid, DisplayMode.Vertial)]
        [UiSonMemberElement]
        public Dictionary<string, string> dict_grid;

        [UiSonCollection(true, CollectionStyle.Stack, DisplayMode.Vertial)]
        [UiSonMemberElement]
        public List<UnparameteredClass> unparam_stack;

        [UiSonCollection(true, CollectionStyle.Grid, DisplayMode.Vertial)]
        [UiSonMemberElement]
        public List<UnparameteredClass> unparam_grid;
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
