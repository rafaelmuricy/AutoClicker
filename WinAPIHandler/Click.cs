namespace WinAPIHandler;

public class Click
{
    public string ID = Guid.NewGuid().ToString().Replace("-", "_");
    public ExternalMethods.POINT point;
    public int delay = 10;
    public int PID;
    public string process = string.Empty;
    public string windowTitle = string.Empty;
    public bool isRunning = false;
    public DateTime lastClick = DateTime.MinValue;
}
