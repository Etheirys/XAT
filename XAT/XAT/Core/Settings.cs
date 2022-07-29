using System;
using System.ComponentModel;

namespace XAT.Core;

[Serializable]
public class Settings : INotifyPropertyChanged
{
#pragma warning disable CS0067
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

    public string GamePath { get; set; } = string.Empty;
}
