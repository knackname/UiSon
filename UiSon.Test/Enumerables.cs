// by Cameron Gale 2022

using System.Collections.Generic;
using UiSon.Attribute;

namespace UiSon.Test
{
    public class Enumerables
    {
        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Grid)]
        [UiSonTextEditUi]
        public List<string> grid;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Grid)]
        [UiSonTextEditUi]
        public List<List<string>> nestedGrid;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Stack)]
        [UiSonTextEditUi]
        public List<string> collection;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Stack)]
        [UiSonTextEditUi]
        public List<List<string>> nestedCollection;

        [UiSonGenericEnumerable]
        [UiSonTextEditUi]
        public List<List<string>> inited_nestedStrangList = new List<List<string>>() { new List<string>() { "A", "B", "C" }, new List<string>() { "1", "2", "3" } };

        [UiSonGenericEnumerable]
        [UiSonTextEditUi]
        public int int_with_enumerable_att;

        [UiSonGenericEnumerable]
        [UiSonTextEditUi]
        public string string_with_enumerable_att;

        [UiSonGenericEnumerable]
        [UiSonTextEditUi]
        public List<ParameteredClass> parametered_members;

        [UiSonGenericEnumerable]
        [UiSonMemberElement]
        public List<SelfRef> selfRefing_members;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Grid)]
        [UiSonMemberElement]
        public List<SelfRef> selfRefing_members_grid;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Stack)]
        [UiSonMemberElement]
        public Dictionary<string,string> dict_stack;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Grid)]
        [UiSonMemberElement]
        public Dictionary<string, string> dict_grid;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Stack)]
        [UiSonMemberElement]
        public List<UnparameteredClass> unparam_stack;

        [UiSonGenericEnumerable(true, DisplayMode.Vertial, CollectionStyle.Grid)]
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
