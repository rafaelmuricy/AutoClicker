namespace AutoClicker;

internal class Click
{
    public Guid ID = Guid.NewGuid();
    public ExternalMethods.POINT point;
    public int delay = 10;
    public int PID;
    public string process = string.Empty;
    public string windowTitle = string.Empty;
    public bool active = false;
    public DateTime lastClick = DateTime.MinValue;
}
