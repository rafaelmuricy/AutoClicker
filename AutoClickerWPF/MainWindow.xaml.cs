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

                VM.Clock = DateTime.Now.ToString("HH:mm:ss");
            }
        });
    }

    void updatePanel()
    {
        foreach (var item in VM.CLICKS)
        {
            if (item.isRunning)
            {
                Dispatcher.BeginInvoke(async () =>
                {
                    panel1.Children.OfType<Label>().First(x => x.Name == $"lblLeft_{item.ID}").Content = $"Time until click: {item.delay - (DateTime.Now - item.lastClick).Seconds}s";
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
            point = point,
            delay = 10,
            PID = PID,
            process = Process.GetProcessById(PID).ProcessName,
            windowTitle = Process.GetProcessById(PID).MainWindowTitle,
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
            Name = $"lbl_{clk.ID}",
            Content = $"Title: {clk.windowTitle}\nProcess: {clk.process}\nPID: {clk.PID}\nX: {clk.point.x}; Y: {clk.point.y}",
        };
        panel1.Children.Add(label);
        

        //delay label
        var delayLabel = new Label
        {
            Name = $"lblDelay_{clk.ID}",
            Content = $"Delay: {clk.delay}",
        };
        panel1.Children.Add(delayLabel);

        //time left label
        var timeLeftLabel = new Label
        {
            Name = $"lblLeft_{clk.ID}",
            Content = $"Time until click: ",
        };
        panel1.Children.Add(timeLeftLabel);

        var sidePanel1 = new UniformGrid
        {
            Name = $"panel1_{clk.ID}",
            Rows = 1,
            Columns = 2
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
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            if (click?.delay > 0)
            {
                click?.delay -= 1;
            }
            panel1.Children.OfType<Label>().First(x => x.Name == $"lblDelay_{clk.ID}").Content = $"Delay: {click!.delay}";
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
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            click?.delay += 1;
            panel1.Children.OfType<Label>().First(x => x.Name == $"lblDelay_{clk.ID}").Content = $"Delay: {click!.delay}";
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
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
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
            Content= "X",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.3)
        };
        removeButton.Click += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            VM.CLICKS.Remove(click!);
            removeClickFromPanel(click!);
        };
        panel1.Children.Add(removeButton);
    }

    void removeClickFromPanel(Click clk)
    {
        var controls = panel1.Children.OfType<FrameworkElement>().Where(x => x.Name.Contains(clk.ID.ToString())).ToList();

        var panelChildren = panel1.Children;
        for (int i = panelChildren.Count - 1; i >= 0; i--)
        {
            if ((panelChildren[i] as FrameworkElement).Name.Contains(clk.ID))
            {
                panel1.Children.RemoveAt(i);
            }
        }
    }

    private void btStartStop_Click(object sender, RoutedEventArgs e)
    {
        if (btStartStop.Content.ToString() == "Start All")
        {
            VM.CLICKS.ForEach(c => c.isRunning = true);
            btStartStop.Content = "Stop All";
        }
        else
        {
            VM.CLICKS.ForEach(c => c.isRunning = false);
            btStartStop.Content = "Start All";
        }
    }
}