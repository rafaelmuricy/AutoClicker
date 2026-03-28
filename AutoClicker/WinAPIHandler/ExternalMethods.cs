using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinAPIHandler;

public partial class ExternalMethods
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static IntPtr _hookID = IntPtr.Zero;
    public static LowLevelMouseProc _proc;


    // Hook
    public static IntPtr SetHook(LowLevelMouseProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        return SetWindowsHookEx(WH_MOUSE_LL, proc,
            GetModuleHandle(curModule.ModuleName), 0);
    }

    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;

    public static Action<POINT, int>? CaptureCallback { set; get; }
    public static IntPtr LowCaptureClickPosition(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
        {
            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            Debug.WriteLine($"X: {hookStruct.pt.x}; Y: {hookStruct.pt.y}");

            IntPtr hWnd = WindowFromPoint(hookStruct.pt);

            GetWindowThreadProcessId(hWnd, out uint processId);

            var process = Process.GetProcessById((int)processId);

            string nome = process.ProcessName;


            // Remove hook imediatamente
            UnhookWindowsHookEx(_hookID);

            //Application.OpenForms[0]?.BeginInvoke(new Action(() =>
            //{
            //    Form1 form = (Form1)Application.OpenForms[0]!;

            //    form.CaptureCallback(hookStruct.pt, (int)processId);

            //    //form.Controls["label1"]?.Text = $"X: {hookStruct.pt.x}; Y: {hookStruct.pt.y}";
            //    //form.Controls["label2"]?.Text = $"X: {hookStruct.pt.x}; Y: {hookStruct.pt.y}";
            //    //form.position((int)processId, hookStruct);
            //}));

            if (CaptureCallback is not null)
            {
                CaptureCallback(hookStruct.pt, (int)processId);
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public async static void MoveMouseClickAndReturn(POINT point)
    {
        Debug.WriteLine("Entrei");
        //mouse original position
        // Salva posição atual
        GetCursorPos(out POINT originalPosition);


        // Move para o destino
        Debug.WriteLine($"[{DateTime.Now}] setei");
        SetCursorPos(point.x, point.y);
        // Clique
        Debug.WriteLine($"[{DateTime.Now}] clickei");

        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);

        await Task.Delay(1); // Aguarda um curto período para garantir que o cursor se mova

        // retorna
        Debug.WriteLine($"[{DateTime.Now}] voltei");
        try
        {
            SetCursorPos(originalPosition.x, originalPosition.y);
        }
        catch (Exception ex )
        {
            Debug.WriteLine(ex.Message);
            
        }
        
    }

    // WinAPI

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
        public int X
        {
            get => x;
        }
        public int Y
        {
            get => y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(POINT Point);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

}
