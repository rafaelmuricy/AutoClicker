using Microsoft.Extensions.DependencyInjection;

namespace AutoClickerMaui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }


    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell())
        {
            Width = 262,
            Height = 489
        };

        return window;
    }
}