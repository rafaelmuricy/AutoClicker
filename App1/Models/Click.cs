using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace App1.Models;

public partial class Click: ObservableObject
{
    [ObservableProperty]
    public partial Guid ID { set; get; }
    
    [ObservableProperty]
    public partial ExternalMethods.POINT Point { set; get; }

    [ObservableProperty]
    public partial int Delay { set; get; } = 10;

    [ObservableProperty]
    public partial int PID { set; get; }

    [ObservableProperty] 
    public partial string process { set; get; }

    [ObservableProperty] 
    public partial string windowTitle { set; get; }

    [ObservableProperty]
    public partial bool isRunning { set; get; } = true;

    [ObservableProperty]
    public partial DateTime lastClick { set; get; }
}
