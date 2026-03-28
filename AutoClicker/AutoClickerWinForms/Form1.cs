using System.Diagnostics;
using WinAPIHandler;

namespace AutoClicker;

public partial class Form1 : Form
{
    static List<Click> CLICKS = new();

    public Form1()
    {
        InitializeComponent();
        worker();
    }

    void worker()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(300));

                foreach (var item in CLICKS)
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

                this.BeginInvoke(() =>
                {
                    updatePanel();

                });
            }
        });
    }

    void updatePanel()
    {
        foreach (var item in CLICKS)
        {
            if (item.IsRunning)
            {
                panel1.Controls[$"lblLeft_{item.Id}"]?.Text = $"Time until click: {item.Delay - (DateTime.Now - item.LastClick).Seconds}s";
            }
        }
        lblClock.Text = DateTime.Now.ToString("HH:mm:ss");
    }

    public void positionWindow(int pid, ExternalMethods.POINT mousePosition)
    {
        var process = Process.GetProcessById(pid);
        var processWindowTitle = process.MainWindowTitle;

        IntPtr handle = process.MainWindowHandle;

        if (ExternalMethods.GetWindowRect(handle, out ExternalMethods.RECT rect))
        {

            Debug.WriteLine($"X: {rect.Left}");
            Debug.WriteLine($"Y: {rect.Top}");
            Debug.WriteLine($"Width: {rect.Right - rect.Left}");
            Debug.WriteLine($"Height: {rect.Bottom - rect.Top}");
        }
        else
        {
            Debug.WriteLine("Error.");
        }

        this.Location = new Point(rect.Right, rect.Top);
        panel1.Visible = true;
    }


    private void btCapture_Click(object sender, EventArgs e)
    {
        btCapture.Enabled = false;
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

        if (!CLICKS.Any())
        {
            positionWindow(click.Pid, click.Point);
        }

        CLICKS.Add(click);

        //reset form for future captures
        btCapture.Enabled = true;
        btCapture.Text = "Capture click";

        panel1.Controls.Clear();
        foreach (var item in CLICKS)
        {
            addClickToPanel(item);
        }
    }

    ControlAttributes getLastControlLocation()
    {
        ControlAttributes lastControlAttr = new();

        if (panel1.Controls.Count > 0)
        {
            var lastControl = panel1.Controls[panel1.Controls.Count - 1];
            lastControlAttr.location = lastControl.Location;
            lastControlAttr.height = lastControl.Height;
            lastControlAttr.width = lastControl.Width;
            lastControlAttr.controlText = lastControl.Text;
        }

        return lastControlAttr;
    }

    void addClickToPanel(Click clk)
    {
        //label
        var lastControlLocation = getLastControlLocation();
        var label = new System.Windows.Forms.Label
        {
            Name = $"lbl_{clk.Id}",
            Text = $"Title: {clk.WindowTitle}\nProcess: {clk.Process}\nPID: {clk.Pid}\nX: {clk.Point.x}; Y: {clk.Point.y}",
            AutoSize = true,
            Location = new Point(0, lastControlLocation.location.Y + lastControlLocation.height),
        };
        panel1.Controls.Add(label);
        if (label.Width > panel1.Width)
        {
            panel1.Width = label.Width;
        }

        //delay label
        lastControlLocation = getLastControlLocation();
        var delayLabel = new System.Windows.Forms.Label
        {
            Name = $"lblDelay_{clk.Id}",
            Text = $"Delay: {clk.Delay}",
            AutoSize = true,
            Location = new Point(0, lastControlLocation.location.Y + lastControlLocation.height),
        };
        panel1.Controls.Add(delayLabel);

        //time left label
        lastControlLocation = getLastControlLocation();
        var timeLeftLabel = new System.Windows.Forms.Label
        {
            Name = $"lblLeft_{clk.Id}",
            Text = $"Time until click: ",
            AutoSize = true,
            Location = new Point(0, lastControlLocation.location.Y + lastControlLocation.height),
        };
        panel1.Controls.Add(timeLeftLabel);

        //minus button
        lastControlLocation = getLastControlLocation();
        var minusButton = new Button
        {
            Name = $"btMinus_{clk.Id}",
            Text = "-",
            AutoSize = true,
            Location = new Point(0, lastControlLocation.height + lastControlLocation.location.Y),
            Width = (int)(panel1.Width * 0.5)
        };
        minusButton.Click += (s, e) =>
        {
            var click = CLICKS.Find(c => c.Id == clk.Id);
            if (click?.Delay > 0)
            {
                click?.Delay -= 1;
            }
            panel1.Controls[$"lblDelay_{clk.Id}"]!.Text = $"Delay: {click!.Delay}";
        };
        panel1.Controls.Add(minusButton);


        //plus button
        lastControlLocation = getLastControlLocation();
        var plusButton = new Button
        {
            Name = $"btPlus_{clk.Id}",
            Text = "+",
            AutoSize = true,
            Location = new Point(lastControlLocation.width, lastControlLocation.location.Y),
            Width = (int)(panel1.Width * 0.5)
        };
        plusButton.Click += (s, e) =>
        {
            var click = CLICKS.Find(c => c.Id == clk.Id);
            click?.Delay += 1;
            panel1.Controls[$"lblDelay_{clk.Id}"]!.Text = $"Delay: {click!.Delay}";
        };
        panel1.Controls.Add(plusButton);

        //start stop button
        lastControlLocation = getLastControlLocation();
        var startStopButton = new Button
        {
            Name = $"btStartStop_{clk.Id}",
            Text = "Start",
            AutoSize = true,
            Margin = new Padding(5),
            Location = new Point(0, lastControlLocation.location.Y + lastControlLocation.height),
            Width = (int)(panel1.Width * 0.7)
        };
        startStopButton.Click += (s, e) =>
        {
            var click = CLICKS.Find(c => c.Id == clk.Id);
            click!.IsRunning = !click.IsRunning;
            if (click.IsRunning)
            {
                panel1.Controls[$"btStartStop_{clk.Id}"]?.Text = "Stop";
            }
            else
            {
                panel1.Controls[$"btStartStop_{clk.Id}"]?.Text = "Start";
            }
        };
        panel1.Controls.Add(startStopButton);

        //remove button
        lastControlLocation = getLastControlLocation();
        var removeButton = new Button
        {
            Name = $"btRemove_{clk.Id}",
            Text = "X",
            AutoSize = true,
            Margin = new Padding(5),
            Location = new Point(lastControlLocation.width, lastControlLocation.location.Y),
            Width = (int)(panel1.Width * 0.3)
        };
        removeButton.Click += (s, e) =>
        {
            var click = CLICKS.Find(c => c.Id == clk.Id);
            CLICKS.Remove(click!);
            removeClickFromPanel(click!);
        };
        panel1.Controls.Add(removeButton);
    }
    void removeClickFromPanel(Click clk)
    {
        var controls = panel1.Controls.OfType<Control>().Where(x => x.Name.Contains(clk.Id.ToString())).ToList();

        foreach (Control item in controls)
        {
            panel1.Controls.Remove(item);
        }

        //reset positions after removal
        panel1.Controls.Clear();
        foreach (var item in CLICKS)
        {
            addClickToPanel(item);
        }
    }


    private void btStartStop_Click(object sender, EventArgs e)
    {
        if (btStartStop.Text == "Start All")
        {
            CLICKS.ForEach(c => c.IsRunning = true);
        }
        else
        {
            CLICKS.ForEach(c => c.IsRunning = false);
        }


        btStartStop.Text = btStartStop.Text == "Start All" ? "Stop All" : "Start All";
    }
}