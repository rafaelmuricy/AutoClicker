using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using WinAPIHandler;

namespace AutoClickerWPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
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

                foreach (var item in VM.CLICKS)
                {
                    if (item.IsRunning)
                    {
                        if ((DateTime.Now - item.LastClick).Seconds >= item.Delay)
                        {
                            ExternalMethods.MoveMouseClickAndReturn(item.Point);
                            item.LastClick = DateTime.Now;
                        }
                    }
                }
                
                updatePanel();

                VM.Clock = DateTime.Now.ToString("HH:mm:ss");
            }
        });
    }

    void updatePanel()
    {
        foreach (var item in VM.CLICKS)
        {
            if (item.IsRunning)
            {
                Dispatcher.BeginInvoke(async () =>
                {
                    panel1.Children.OfType<Label>().First(x => x.Name == $"lblLeft_{item.Id}").Content = $"Time until click: {item.Delay - (DateTime.Now - item.LastClick).Seconds}s";
                });
            }
        }
        VM.Clock = DateTime.Now.ToString("HH:mm:ss");
    }

    private void btCapture_Click(object sender, RoutedEventArgs e)
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
            Point = point,
            Delay = 10,
            Pid = PID,
            Process = Process.GetProcessById(PID).ProcessName,
            WindowTitle = Process.GetProcessById(PID).MainWindowTitle,
        };

        if (!VM.CLICKS.Any())
        {
            //positionWindow(click.PID, click.point);
        }

        VM.CLICKS.Add(click);

        //reset form for future captures
        btCapture.IsEnabled = true;
        btCapture.Content = "Capture click";

        panel1.Children.Clear();
        foreach (var item in VM.CLICKS)
        {
            addClickToPanel(item);
        }
    }

    void addClickToPanel(Click clk)
    {
        //label
        var label = new Label
        {
            Name = $"lbl_{clk.Id}",
            Content = $"Title: {clk.WindowTitle}\nProcess: {clk.Process}\nPID: {clk.Pid}\nX: {clk.Point.x}; Y: {clk.Point.y}",
        };
        panel1.Children.Add(label);
        

        //delay label
        var delayLabel = new Label
        {
            Name = $"lblDelay_{clk.Id}",
            Content = $"Delay: {clk.Delay}",
        };
        panel1.Children.Add(delayLabel);

        //time left label
        var timeLeftLabel = new Label
        {
            Name = $"lblLeft_{clk.Id}",
            Content = $"Time until click: ",
        };
        panel1.Children.Add(timeLeftLabel);

        var sidePanel1 = new UniformGrid
        {
            Name = $"panel1_{clk.Id}",
            Rows = 1,
            Columns = 2
        };

        //minus button
        var minusButton = new Button
        {
            Name = $"btMinus_{clk.Id}",
            Content = "-",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        minusButton.Click += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            if (click?.Delay > 0)
            {
                click?.Delay -= 1;
            }
            panel1.Children.OfType<Label>().First(x => x.Name == $"lblDelay_{clk.Id}").Content = $"Delay: {click!.Delay}";
        };
        sidePanel1.Children.Add(minusButton);


        //plus button
        var plusButton = new Button
        {
            Name = $"btPlus_{clk.Id}",
            Content = "+",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        plusButton.Click += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            click?.Delay += 1;
            panel1.Children.OfType<Label>().First(x => x.Name == $"lblDelay_{clk.Id}").Content = $"Delay: {click!.Delay}";
        };
        sidePanel1.Children.Add(plusButton);

        panel1.Children.Add(sidePanel1);

        //start stop button
        var startStopButton = new Button
        {
            Name = $"btStartStop_{clk.Id}",
            Content = "Start",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.7)
        };
        startStopButton.Click += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            click!.IsRunning = !click.IsRunning;
            if (click.IsRunning)
            {
                panel1.Children.OfType<Button>().First(x => x.Name == $"btStartStop_{clk.Id}").Content = "Stop";
            }
            else
            {
                panel1.Children.OfType<Button>().First(x => x.Name == $"btStartStop_{clk.Id}").Content = "Start";
            }
        };
        panel1.Children.Add(startStopButton);

        //remove button
        var removeButton = new Button
        {
            Name = $"btRemove_{clk.Id}",
            Content= "X",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.3)
        };
        removeButton.Click += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            VM.CLICKS.Remove(click!);
            removeClickFromPanel(click!);
        };
        panel1.Children.Add(removeButton);
    }

    void removeClickFromPanel(Click clk)
    {
        var controls = panel1.Children.OfType<FrameworkElement>().Where(x => x.Name.Contains(clk.Id.ToString())).ToList();

        var panelChildren = panel1.Children;
        for (int i = panelChildren.Count - 1; i >= 0; i--)
        {
            if ((panelChildren[i] as FrameworkElement).Name.Contains(clk.Id))
            {
                panel1.Children.RemoveAt(i);
            }
        }
    }

    private void btStartStop_Click(object sender, RoutedEventArgs e)
    {
        if (btStartStop.Content.ToString() == "Start All")
        {
            VM.CLICKS.ForEach(c => c.IsRunning = true);
            btStartStop.Content = "Stop All";
        }
        else
        {
            VM.CLICKS.ForEach(c => c.IsRunning = false);
            btStartStop.Content = "Start All";
        }
    }
}