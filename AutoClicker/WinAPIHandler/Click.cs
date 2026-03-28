using CommunityToolkit.Mvvm.ComponentModel;

namespace WinAPIHandler;

public partial class Click : ObservableObject
{
    [ObservableProperty]
    string id = Guid.NewGuid().ToString().Replace("-", "_");
    
    [ObservableProperty]
    ExternalMethods.POINT point;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimeLeft))]
    int delay = 10;
    
    [ObservableProperty]
    int pid;
    
    [ObservableProperty]
    string process = string.Empty;
    
    [ObservableProperty]
    string windowTitle = string.Empty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BtStartStopLabel))]
    bool isRunning = false;
    
    [ObservableProperty]
    DateTime lastClick;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TimeLeft))]
    DateTime currentTime;

    public int TimeLeft
    {
        get
        {
            if (!IsRunning)
                return 0;

            return Delay - (DateTime.Now - LastClick).Seconds;
        }
    }
    public string BtStartStopLabel { get { return IsRunning ? "Stop" : "Start"; } }
}
