using System.Diagnostics;
using WinAPIHandler;

namespace AutoClickerMaui;

public partial class MainPage : ContentPage
{
    MainPageVM VM = new();

    public MainPage()
    {
        InitializeComponent();
        BindingContext = VM;
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
                Dispatcher.DispatchAsync(async () =>
                {
                    panel1.Children.OfType<Label>().First(x => x.AutomationId == $"lblLeft_{item.Id}").Text = $"Time until click: {item.Delay - (DateTime.Now - item.LastClick).Seconds}s";
                });
            }
        }
        VM.Clock = DateTime.Now.ToString("HH:mm:ss");
    }

    //private void OnCounterClicked(object? sender, EventArgs e)
    //{
    //    count++;

    //    if (count == 1)
    //        CounterBtn.Text = $"Clicked {count} time";
    //    else
    //        CounterBtn.Text = $"Clicked {count} times";

    //    SemanticScreenReader.Announce(CounterBtn.Text);
    //}

    private void btCapture_Clicked(object sender, EventArgs e)
    {
        btCapture.IsEnabled = false;
        btCapture.Text = "Capturing...";
        
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
        btCapture.Text = "Capture click";

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
            AutomationId = $"lbl_{clk.Id}",
            Text = $"Title: {clk.WindowTitle}\nProcess: {clk.Process}\nPID: {clk.Pid}\nX: {clk.Point.x}; Y: {clk.Point.y}",
        };
        panel1.Children.Add(label);


        //delay label
        var delayLabel = new Label
        {
            AutomationId = $"lblDelay_{clk.Id}",
            Text = $"Delay: {clk.Delay}",
        };
        panel1.Children.Add(delayLabel);

        //time left label
        var timeLeftLabel = new Label
        {
            AutomationId = $"lblLeft_{clk.Id}",
            Text = $"Time until click: ",
        };
        panel1.Children.Add(timeLeftLabel);

        var sidePanel1 = new StackLayout
        {
            AutomationId = $"panel1_{clk.Id}",
            Orientation = StackOrientation.Horizontal,
        };

        //minus button
        var minusButton = new Button
        {
            AutomationId = $"btMinus_{clk.Id}",
            Text = "-",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        minusButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            if (click?.Delay > 0)
            {
                click?.Delay -= 1;
            }
            panel1.Children.OfType<Label>().First(x => x.AutomationId == $"lblDelay_{clk.Id}").Text = $"Delay: {click!.Delay}";
        };
        sidePanel1.Children.Add(minusButton);


        //plus button
        var plusButton = new Button
        {
            AutomationId = $"btPlus_{clk.Id}",
            Text = "+",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        plusButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            click?.Delay += 1;
            panel1.Children.OfType<Label>().First(x => x.AutomationId == $"lblDelay_{clk.Id}").Text = $"Delay: {click!.Delay}";
        };
        sidePanel1.Children.Add(plusButton);

        panel1.Children.Add(sidePanel1);

        //start stop button
        var startStopButton = new Button
        {
            AutomationId = $"btStartStop_{clk.Id}",
            Text = "Start",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.7)
        };
        startStopButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            click!.IsRunning = !click.IsRunning;
            if (click.IsRunning)
            {
                panel1.Children.OfType<Button>().First(x => x.AutomationId == $"btStartStop_{clk.Id}").Text = "Stop";
            }
            else
            {
                panel1.Children.OfType<Button>().First(x => x.AutomationId == $"btStartStop_{clk.Id}").Text = "Start";
            }
        };
        panel1.Children.Add(startStopButton);

        //remove button
        var removeButton = new Button
        {
            AutomationId = $"btRemove_{clk.Id}",
            Text = "X",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.3)
        };
        removeButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.Id == clk.Id);
            VM.CLICKS.Remove(click!);
            removeClickFromPanel(click!);
        };
        panel1.Children.Add(removeButton);
    }

    void removeClickFromPanel(Click clk)
    {
        var controls = panel1.Children.OfType<View>().Where(x => x.AutomationId.Contains(clk.Id.ToString())).ToList();

        var panelChildren = panel1.Children;
        for (int i = panelChildren.Count - 1; i >= 0; i--)
        {
            if ((panelChildren[i] as View).AutomationId.Contains(clk.Id))
            {
                panel1.Children.RemoveAt(i);
            }
        }
    }

    private void btStartStop_Click(object sender, EventArgs e)
    {
        if (btStartStop.Text.ToString() == "Start All")
        {
            VM.CLICKS.ForEach(c => c.IsRunning = true);
            btStartStop.Text = "Stop All";
        }
        else
        {
            VM.CLICKS.ForEach(c => c.IsRunning = false);
            btStartStop.Text = "Start All";
        }
    }
}
