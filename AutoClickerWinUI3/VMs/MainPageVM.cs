using App1.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.System;

namespace App1.VMs;

public partial class MainPageVM : ObservableObject
{
    DispatcherQueueTimer timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
    [ObservableProperty]
    public partial string CurrentTime { get; set; } = string.Empty;
    public List<Click> CLICKS = new();
    public MainPageVM()
    {
        timer.Interval = TimeSpan.FromSeconds(1);
        int ticks = 0;
        timer.Tick += (s, e) =>
        {
            CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            //Debug.WriteLine(ticks + "- " + CurrentTime);
            ticks++;
        };
        timer.Start();
    }

    
}
