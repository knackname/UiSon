// UiSon, by Cameron Gale 2021

using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace UiSon.Element
{
    /// <summary>
    /// Text edit with a label
    /// </summary>
    public class TextEditElement : StringElement
    {
        public TextEditElement(string name, int priority, MemberInfo info, string defaultValue, string regexValidation)
            :base(name, priority, info, regexValidation)
        {
            Value = defaultValue;
        }

        public override IEnumerable<DataGridColumn> GenerateColumns(string path)
        {
            var columns = new List<DataGridColumn>();

            var valCol = new DataGridTextColumn();
            valCol.Header = Name;

            var valBinding = new Binding(path + ".Value");
            valBinding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;

            valCol.Binding = valBinding;
            columns.Add(valCol);

            return columns;
        }
    }
}
