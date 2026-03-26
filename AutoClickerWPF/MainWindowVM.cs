using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using WinAPIHandler;

namespace AutoClickerWPF;

public partial class MainWindowVM : ObservableObject
{
    [ObservableProperty]
    public partial string Clock { set; get; }

    [ObservableProperty]
    public partial List<Click> CLICKS { set; get; } = new();
}
