using App1.Models;
using App1.VMs;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace App1;
/// <summary>
/// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPageVM VM { get; } = new();
    public MainPage()
    {
        InitializeComponent();
    }

    private void btCapture_Click(object sender, RoutedEventArgs e)
    {
        btCapture.IsEnabled = false;
        btCapture.Content = "Capturing...";

        ExternalMethods._proc = ExternalMethods.LowCaptureClickPosition;
        ExternalMethods.CaptureCallback = CaptureCallback;
        ExternalMethods._hookID = ExternalMethods.SetHook(ExternalMethods._proc);
        
        
        
        
        panel1.Children.Add(new TextBlock { Text = "Hello World" });
    }

    public void CaptureCallback(ExternalMethods.POINT point, int PID)
    {
        Click click = new Click
        {
            Point = point,
            Delay = 10,
            PID = PID,
            process = System.Diagnostics.Process.GetProcessById(PID).ProcessName,
            windowTitle = System.Diagnostics.Process.GetProcessById(PID).MainWindowTitle,
        };
        if (!VM.CLICKS.Any())
        {
            //REPENSAR
            //positionWindow(click.PID, click.point);
        }
        VM.CLICKS.Add(click);
        updatePanel();
        //reset form for future captures
        btCapture.IsEnabled = true;
        btCapture.Content = "Capture click";
    }


    void updatePanel()
    {
        foreach (var item in VM.CLICKS)
        {
            //label
            var label = new TextBlock
            {
                Name = $"lbl_{item.ID}",
                Text = $"Title: {item.windowTitle}\nProcess: {item.process}\nPID: {item.PID}\nX: {item.Point.x}; Y: {item.Point.y}",
            };
            panel1.Children.Add(label);
            //if (label.Width > panel1.Width)
            //{
            //    panel1.Width = label.Width;
            //}

            //delay label
            var delayLabel = new TextBlock
            {
                Name = $"lblDelay_{item.ID}",
                Text = $"Delay: {item.Delay}",
            };
            panel1.Children.Add(delayLabel);

            //time left label
            var timeLeftLabel = new TextBlock
            {
                Name = $"lblLeft_{item.ID}",
                Text = $"Time until click: ",
            };
            panel1.Children.Add(timeLeftLabel);

            //minus button
            var minusButton = new Button
            {
                Name = $"btMinus_{item.ID}",
                Content = "-",
                //Width = (int)(panel1.Width * 0.5)
            };
            minusButton.Click += (s, e) =>
            {
                var click = VM.CLICKS.Find(c => c.ID == item.ID);
                if (click?.Delay > 0)
                {
                    click?.Delay -= 1;
                }
                panel1.Children.OfType<TextBlock>().FirstOrDefault(x => x.Name == $"lblDelay_{item.ID}")?.Text = $"Delay: {click!.Delay}";
            };
            panel1.Children.Add(minusButton);


            //plus button
            var plusButton = new Button
            {
                Name = $"btPlus_{item.ID}",
                Content = "+",
                //Width = (int)(panel1.Width * 0.5)
            };
            plusButton.Click += (s, e) =>
            {
                var click = VM.CLICKS.Find(c => c.ID == item.ID);
                click?.Delay += 1;
                panel1.Children.OfType<TextBlock>().FirstOrDefault(x => x.Name == $"lblDelay_{item.ID}")?.Text = $"Delay: {click!.Delay}";
            };
            panel1.Children.Add(plusButton);

            //start stop button
            var startStopButton = new Button
            {
                Name = $"btStartStop_{item.ID}",
                Content = "Start",
                Margin = new Thickness(5),
                //Location = new Point(0, lastControlLocation.location.Y + lastControlLocation.height),
                //Width = (int)(panel1.Width * 0.7)
            };
            startStopButton.Click += (s, e) =>
            {
                var click = VM.CLICKS.Find(c => c.ID == item.ID);
                click!.isRunning = !click.isRunning;
                if (click.isRunning)
                {                    
                    panel1.Children.OfType<Button>().FirstOrDefault(x => x.Name == $"btStartStop_{item.ID}")?.Content = "Stop";
                }
                else
                {
                    panel1.Children.OfType<Button>().FirstOrDefault(x => x.Name == $"btStartStop_{item.ID}")?.Content = "Start";
                    
                }
            };
            panel1.Children.Add(startStopButton);

            //remove button
            var removeButton = new Button
            {
                Name = $"btRemove_{item.ID}",
                Content = "X",
                Margin = new Thickness(5),
                //Location = new Point(lastControlLocation.width, lastControlLocation.location.Y),
                //Width = (int)(panel1.Width * 0.3)
            };
            removeButton.Click += (s, e) =>
            {
                var click = VM.CLICKS.Find(c => c.ID == item.ID);
                VM.CLICKS.Remove(click!);
                removeClickFromPanel(click!);
            };
            panel1.Children.Add(removeButton);
        }

        void removeClickFromPanel(Click clk)
        {
            var controls = panel1.Children.OfType<FrameworkElement>().Where(x => x.Name.Contains(clk.ID.ToString())).ToList();

            foreach (FrameworkElement item in controls)
            {
                panel1.Children.Remove(item);
            }

            //reset positions after removal
            //panel1.Controls.Clear();
            //foreach (var item in CLICKS)
            //{
            //    addClickToPanel(item);
            //}
        }

    }
}
