namespace AutoClickerUno.Presentation;

public partial class MainViewModel : ObservableObject
{
    private INavigator _navigator;

    [ObservableProperty]
    private string? name;
    
    [ObservableProperty]
    private string? btCaptureLabel = "Capture Click";

    public MainViewModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;
        Title = "Main";
        Title += $" - {localizer["ApplicationName"]}";
        Title += $" - {appInfo?.Value?.Environment}";
        Capture = new AsyncRelayCommand(CaptureClick);
    }
    public string? Title { get; }

    public ICommand Capture { get; }

    private async Task CaptureClick()
    {
        BtCaptureLabel = "Capturing...";
    }

    private async Task GoToSecondView()
    {
        await _navigator.NavigateViewModelAsync<SecondViewModel>(this, data: new Entity(Name!));
    }

}
