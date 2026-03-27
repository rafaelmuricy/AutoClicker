using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using WinAPIHandler;

namespace AutoClickerAvalonia.ViewModels;

public partial class MainWindowVM : ObservableObject
{
    [ObservableProperty]
    string clock = "HH:mm:ss";

    [ObservableProperty]
    private List<Click> clicks = new();
}
