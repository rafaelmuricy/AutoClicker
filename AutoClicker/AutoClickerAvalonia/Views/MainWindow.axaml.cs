using AutoClickerAvalonia.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinAPIHandler;

namespace AutoClickerAvalonia.Views;

public partial class MainWindow : Window
{
    MainWindowVM VM = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = VM;
        worker();
    }

    void worker()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300));

                foreach (var item in VM.Clicks)
                {
                    if (item.isRunning)
                    {
                        if ((DateTime.Now - item.lastClick).Seconds >= item.delay)
                        {
                            ExternalMethods.MoveMouseClickAndReturn(item.point);
                            item.lastClick = DateTime.Now;
                        }
                    }
                }

                updatePanel();
            }
        });
    }

    void updatePanel()
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            VM.Clock = DateTime.Now.ToString("HH:mm:ss");
            foreach (var item in VM.Clicks)
            {
                if (item.isRunning)
                {
                    panel1.Children.OfType<TextBlock>().First(x => x.Name == $"lblLeft_{item.ID}").Text = $"Time until click: {item.delay - (DateTime.Now - item.lastClick).Seconds}s";
                }
            }
        });
    }

    private void btCapture_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        btCapture.IsEnabled = false;
        btCapture.Content = "Capturing...";

        ExternalMethods._proc = ExternalMethods.LowCaptureClickPosition;
        ExternalMethods.CaptureCallback = CaptureCallback;
        ExternalMethods._hookID = ExternalMethods.SetHook(ExternalMethods._proc);
    }

    public void CaptureCallback(ExternalMethods.POINT point, int PID)
    {
        Click click = new Click
        {
            point = point,
            delay = 10,
            PID = PID,
            process = Process.GetProcessById(PID).ProcessName,
            windowTitle = Process.GetProcessById(PID).MainWindowTitle,
        };

        if (!VM.Clicks.Any())
        {
            //positionWindow(click.PID, click.point);
        }

        VM.Clicks.Add(click);

        //reset form for future captures
        btCapture.IsEnabled = true;
        btCapture.Content = "Capture click";

        panel1.Children.Clear();
        foreach (var item in VM.Clicks)
        {
            addClickToPanel(item);
        }
    }

    void addClickToPanel(Click clk)
    {
        //label
        var label = new TextBlock
        {
            Name = $"lbl_{clk.ID}",
            Text = $"Title: {clk.windowTitle}\nProcess: {clk.process}\nPID: {clk.PID}\nX: {clk.point.x}; Y: {clk.point.y}",
        };
        panel1.Children.Add(label);


        //delay label
        var delayLabel = new TextBlock
        {
            Name = $"lblDelay_{clk.ID}",
            Text = $"Delay: {clk.delay}",
        };
        panel1.Children.Add(delayLabel);

        //time left label
        var timeLeftLabel = new TextBlock
        {
            Name = $"lblLeft_{clk.ID}",
            Text = $"Time until click: ",
        };
        panel1.Children.Add(timeLeftLabel);

        var sidePanel1 = new StackPanel
        {
            Name = $"panel1_{clk.ID}",
            Orientation = Orientation.Horizontal,
        };

        //minus button
        var minusButton = new Button
        {
            Name = $"btMinus_{clk.ID}",
            Content = "-",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        minusButton.Click += (s, e) =>
        {
            var click = VM.Clicks.Find(c => c.ID == clk.ID);
            if (click?.delay > 0)
            {
                click?.delay -= 1;
            }
            panel1.Children.OfType<TextBlock>().First(x => x.Name == $"lblDelay_{clk.ID}").Text = $"Delay: {click!.delay}";
        };
        sidePanel1.Children.Add(minusButton);


        //plus button
        var plusButton = new Button
        {
            Name = $"btPlus_{clk.ID}",
            Content = "+",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        plusButton.Click += (s, e) =>
        {
            var click = VM.Clicks.Find(c => c.ID == clk.ID);
            click?.delay += 1;
            panel1.Children.OfType<TextBlock>().First(x => x.Name == $"lblDelay_{clk.ID}").Text = $"Delay: {click!.delay}";
        };
        sidePanel1.Children.Add(plusButton);

        panel1.Children.Add(sidePanel1);

        //start stop button
        var startStopButton = new Button
        {
            Name = $"btStartStop_{clk.ID}",
            Content = "Start",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.7)
        };
        startStopButton.Click += (s, e) =>
        {
            var click = VM.Clicks.Find(c => c.ID == clk.ID);
            click!.isRunning = !click.isRunning;
            if (click.isRunning)
            {
                panel1.Children.OfType<Button>().First(x => x.Name == $"btStartStop_{clk.ID}").Content = "Stop";
            }
            else
            {
                panel1.Children.OfType<Button>().First(x => x.Name == $"btStartStop_{clk.ID}").Content = "Start";
            }
        };
        panel1.Children.Add(startStopButton);

        //remove button
        var removeButton = new Button
        {
            Name = $"btRemove_{clk.ID}",
            Content = "X",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.3)
        };
        removeButton.Click += (s, e) =>
        {
            var click = VM.Clicks.Find(c => c.ID == clk.ID);
            VM.Clicks.Remove(click!);
            removeClickFromPanel(click!);
        };
        panel1.Children.Add(removeButton);
    }

    void removeClickFromPanel(Click clk)
    {

        var controls = panel1.Children.OfType<Control>().Where(x => x.Name.Contains(clk.ID.ToString())).ToList();

        foreach (Control control in controls)
        {
            panel1.Children.Remove(control);
        }
    }

    private void btStartStop_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (btStartStop.Content.ToString() == "Start All")
        {
            VM.Clicks.ForEach(c => c.isRunning = true);
            btStartStop.Content = "Stop All";
        }
        else
        {
            VM.Clicks.ForEach(c => c.isRunning = false);
            btStartStop.Content = "Start All";
        }
    }
}