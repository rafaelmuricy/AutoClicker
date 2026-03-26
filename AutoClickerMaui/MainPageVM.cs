using CommunityToolkit.Mvvm.ComponentModel;
using WinAPIHandler;

namespace AutoClickerMaui;

public partial class MainPageVM : ObservableObject
{
    [ObservableProperty]
    public partial string Clock { set; get; }

    [ObservableProperty]
    public partial List<Click> CLICKS { set; get; } = new();
}
