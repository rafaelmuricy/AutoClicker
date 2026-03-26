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
                Dispatcher.DispatchAsync(async () =>
                {
                    panel1.Children.OfType<Label>().First(x => x.AutomationId == $"lblLeft_{item.ID}").Text = $"Time until click: {item.delay - (DateTime.Now - item.lastClick).Seconds}s";
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
            AutomationId = $"lbl_{clk.ID}",
            Text = $"Title: {clk.windowTitle}\nProcess: {clk.process}\nPID: {clk.PID}\nX: {clk.point.x}; Y: {clk.point.y}",
        };
        panel1.Children.Add(label);


        //delay label
        var delayLabel = new Label
        {
            AutomationId = $"lblDelay_{clk.ID}",
            Text = $"Delay: {clk.delay}",
        };
        panel1.Children.Add(delayLabel);

        //time left label
        var timeLeftLabel = new Label
        {
            AutomationId = $"lblLeft_{clk.ID}",
            Text = $"Time until click: ",
        };
        panel1.Children.Add(timeLeftLabel);

        var sidePanel1 = new StackLayout
        {
            AutomationId = $"panel1_{clk.ID}",
            Orientation = StackOrientation.Horizontal,
        };

        //minus button
        var minusButton = new Button
        {
            AutomationId = $"btMinus_{clk.ID}",
            Text = "-",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        minusButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            if (click?.delay > 0)
            {
                click?.delay -= 1;
            }
            panel1.Children.OfType<Label>().First(x => x.AutomationId == $"lblDelay_{clk.ID}").Text = $"Delay: {click!.delay}";
        };
        sidePanel1.Children.Add(minusButton);


        //plus button
        var plusButton = new Button
        {
            AutomationId = $"btPlus_{clk.ID}",
            Text = "+",
            Margin = new Thickness(5)
            //Width = (int)(panel1.Width * 0.5)
        };
        plusButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            click?.delay += 1;
            panel1.Children.OfType<Label>().First(x => x.AutomationId == $"lblDelay_{clk.ID}").Text = $"Delay: {click!.delay}";
        };
        sidePanel1.Children.Add(plusButton);

        panel1.Children.Add(sidePanel1);

        //start stop button
        var startStopButton = new Button
        {
            AutomationId = $"btStartStop_{clk.ID}",
            Text = "Start",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.7)
        };
        startStopButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            click!.isRunning = !click.isRunning;
            if (click.isRunning)
            {
                panel1.Children.OfType<Button>().First(x => x.AutomationId == $"btStartStop_{clk.ID}").Text = "Stop";
            }
            else
            {
                panel1.Children.OfType<Button>().First(x => x.AutomationId == $"btStartStop_{clk.ID}").Text = "Start";
            }
        };
        panel1.Children.Add(startStopButton);

        //remove button
        var removeButton = new Button
        {
            AutomationId = $"btRemove_{clk.ID}",
            Text = "X",
            Margin = new Thickness(5),
            //Width = (int)(panel1.Width * 0.3)
        };
        removeButton.Clicked += (s, e) =>
        {
            var click = VM.CLICKS.Find(c => c.ID == clk.ID);
            VM.CLICKS.Remove(click!);
            removeClickFromPanel(click!);
        };
        panel1.Children.Add(removeButton);
    }

    void removeClickFromPanel(Click clk)
    {
        var controls = panel1.Children.OfType<View>().Where(x => x.AutomationId.Contains(clk.ID.ToString())).ToList();

        var panelChildren = panel1.Children;
        for (int i = panelChildren.Count - 1; i >= 0; i--)
        {
            if ((panelChildren[i] as View).AutomationId.Contains(clk.ID))
            {
                panel1.Children.RemoveAt(i);
            }
        }
    }

    private void btStartStop_Click(object sender, EventArgs e)
    {
        if (btStartStop.Text.ToString() == "Start All")
        {
            VM.CLICKS.ForEach(c => c.isRunning = true);
            btStartStop.Text = "Stop All";
        }
        else
        {
            VM.CLICKS.ForEach(c => c.isRunning = false);
            btStartStop.Text = "Start All";
        }
    }
}
