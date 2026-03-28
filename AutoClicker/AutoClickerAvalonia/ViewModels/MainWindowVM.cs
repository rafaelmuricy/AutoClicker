using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using WinAPIHandler;

namespace AutoClickerAvalonia.ViewModels;

public partial class MainWindowVM : ObservableObject
{
    [ObservableProperty]
    private string btCaptureLabel = "Capture Click";
    [ObservableProperty]
    private string btStartStopLabel = "Start All";
    [ObservableProperty]
    private string clock = DateTime.Now.ToString("HH:mm:ss");
    [ObservableProperty]
    private bool btCaptureEnabled = true;

    public ObservableCollection<Click> clicks { get; set; } = new();


    public ICommand Capture { get; }
    public ICommand StartStopAll { get; }
    public IRelayCommand<Click> Minus { get; }
    public IRelayCommand<Click> Plus { get; }
    public IRelayCommand<Click> StartStop { get; }
    public IRelayCommand<Click> Delete { get; }

    public MainWindowVM()
    {
        Capture = new AsyncRelayCommand(CaptureClick);
        StartStopAll = new AsyncRelayCommand(StartStopAllClick);

        Minus = new AsyncRelayCommand<Click>(MinusClick);
        Plus = new AsyncRelayCommand<Click>(PlusClick);
        StartStop = new AsyncRelayCommand<Click>(StartStopClick);
        Delete = new AsyncRelayCommand<Click>(DeleteClick);
        worker();
    }

    void worker()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                Clock = DateTime.Now.ToString("HH:mm:ss");
                await Task.Delay(TimeSpan.FromMilliseconds(300));

                foreach (var item in clicks)
                {
                    if (item.IsRunning)
                    {
                        item.CurrentTime = DateTime.Now;
                        if (item.TimeLeft <= 0)
                        {
                            ExternalMethods.MoveMouseClickAndReturn(item.Point);
                            item.LastClick = DateTime.Now;
                        }
                    }
                }
            }
        });
    }

    private async Task CaptureClick()
    {
        BtCaptureEnabled = false;
        BtCaptureLabel = "Capturing...";

        ExternalMethods._proc = ExternalMethods.LowCaptureClickPosition;
        ExternalMethods.CaptureCallback = CaptureCallback;
        ExternalMethods._hookID = ExternalMethods.SetHook(ExternalMethods._proc);
    }

    public void CaptureCallback(ExternalMethods.POINT point, int PID)
    {
        Click click = new Click
        {
            Point = point,
            Delay = 10,
            Pid = PID,
            Process = Process.GetProcessById(PID).ProcessName,
            WindowTitle = Process.GetProcessById(PID).MainWindowTitle,
        };

        if (!clicks.Any())
        {
            //positionWindow(click.PID, click.point);
        }

        clicks.Add(click);

        //reset form for future captures
        BtCaptureEnabled = true;
        BtCaptureLabel = "Capture click";
    }

    private async Task StartStopAllClick()
    {
        if (BtStartStopLabel == "Start All")
        {
            foreach (var item in clicks)
            {
                item.LastClick = DateTime.Now;
                item.IsRunning = true;
            }
            BtStartStopLabel = "Stop All";
        }
        else
        {
            foreach (var item in clicks)
            {
                item.IsRunning = false;
            }
            BtStartStopLabel = "Start All";
        }
    }

    private async Task MinusClick(Click click)
    {
        click.Delay--;
    }
    private async Task PlusClick(Click click)
    {
        click.Delay++;
    }
    private async Task StartStopClick(Click click)
    {
        click.LastClick = DateTime.Now;
        click.IsRunning = !click.IsRunning;
    }
    private async Task DeleteClick(Click click)
    {
        clicks.Remove(click);
    }
}
