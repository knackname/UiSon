﻿// UiSon 2021, Cameron Gale

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UiSon
{
    public abstract class NPCBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
