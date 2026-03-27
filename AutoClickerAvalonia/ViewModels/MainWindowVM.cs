using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WinAPIHandler;

namespace AutoClickerAvalonia.ViewModels;

public partial class MainWindowVM : ObservableObject
{
    [ObservableProperty]
    string clock;

    //[ObservableProperty]
    public ObservableCollection<Click> Clicks { get; } = new();
}
