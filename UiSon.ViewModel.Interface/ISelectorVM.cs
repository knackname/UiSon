// UiSon, by Cameron Gale 2022

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UiSon.ViewModel.Interface
{
    public interface ISelectorVM
    {
        string Name { get; }
        int Priority { get; }
        string Value { get; }
        IEnumerable<string> Options { get; }
        Brush TextColor { get; }
        Visibility IsNameVisible { get; }
        void Read(object instance);
        void Write(object instance);
        bool SetValue(object value);
        object GetValueAs(Type type);
        IEnumerable<DataGridColumn> GenerateColumns(string path);
    }
}
