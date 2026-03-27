namespace WinAPIHandler;

public class Click
{
    public string ID { get { return Guid.NewGuid().ToString().Replace("-", "_"); } } 
    public ExternalMethods.POINT point { get; set; }
    public int delay = 10;
    public int PID { set; get; }
    public string process = string.Empty;
    public string windowTitle = string.Empty;
    public bool isRunning = false;
    public DateTime lastClick = DateTime.MinValue;
    public int timeLeft;
}
