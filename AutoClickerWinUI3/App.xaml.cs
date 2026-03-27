using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace App1;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        _window = new Window();

        Frame frame = new Frame();
        frame.Navigate(typeof(MainPage));

        _window.Content = frame;
        _window.Activate();
    }
}