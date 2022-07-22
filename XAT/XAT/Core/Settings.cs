using PropertyChanged;
using System;
using System.ComponentModel;

namespace XAT.Core;

[Serializable]
[AddINotifyPropertyChangedInterface]
public class Settings : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
}
