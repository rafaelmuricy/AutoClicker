using AutoClickerAvalonia.ViewModels;
using Avalonia.Controls;

namespace AutoClickerAvalonia.Views;

public partial class MainWindow : Window
{
    MainWindowVM VM = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = VM;
    }
}