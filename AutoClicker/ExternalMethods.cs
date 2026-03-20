using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoClicker;

public partial class ExternalMethods
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

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
        using (var curProcess = Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;

    /// <summary>
    /// Processes low-level mouse hook events and determines whether to handle or pass them to the next hook procedure.
    /// </summary>
    /// <remarks>If the mouse left button is pressed, the method retrieves information about the window and
    /// process under the cursor, removes the hook, and invokes a callback on the application's main form. The method
    /// should be used as a callback for a low-level mouse hook (WH_MOUSE_LL).</remarks>
    /// <param name="nCode">A code that indicates the hook procedure action. If this value is less than zero, the hook procedure must pass
    /// the message to the next hook procedure without further processing.</param>
    /// <param name="wParam">The identifier of the mouse message. Typically corresponds to a Windows message such as WM_LBUTTONDOWN.</param>
    /// <param name="lParam">A pointer to a structure containing information about the mouse event.</param>
    /// <returns>A pointer to the result of the hook processing. Returns the value from the next hook procedure in the chain if
    /// the event is not handled.</returns>
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

            Application.OpenForms[0]?.BeginInvoke(new Action(() =>
            {
                Form1 form = (Form1)Application.OpenForms[0]!;


                //form.Controls["label1"]?.Text = $"X: {hookStruct.pt.x}; Y: {hookStruct.pt.y}";
                form.Controls["label2"]?.Text = $"X: {hookStruct.pt.x}; Y: {hookStruct.pt.y}";
                form.position((int)processId, hookStruct);
            }));
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;
    public async static void MoveMouseClickAndReturn()
    {
        Debug.WriteLine("Entrei");
        //mouse original position
        // Salva posição atual
        GetCursorPos(out POINT originalPosition);


        // Move para o destino
        Debug.WriteLine($"[{DateTime.Now}] setei");
        SetCursorPos(Form1.MouseX, Form1.MouseY);
        // Clique
        Debug.WriteLine($"[{DateTime.Now}] clickei");

        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);

        await Task.Delay(1); // Aguarda um curto período para garantir que o cursor se mova

        // retorna
        Debug.WriteLine($"[{DateTime.Now}] voltei");
        SetCursorPos(originalPosition.x, originalPosition.y);

        /////////TESTE
        return;


        // Pequeno delay (opcional, mas mais seguro)
        Thread.Sleep(1);

        // Clique
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);

        // Delay antes de voltar
        Thread.Sleep(1);

        // Retorna para posição original
        SetCursorPos(originalPosition.x, originalPosition.y);
        // Clique
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);


        // Remove hook imediatamente
        UnhookWindowsHookEx(_hookID);

    }

    // WinAPI

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;
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
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

}
