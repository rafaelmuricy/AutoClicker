using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AutoClicker;

public partial class Form1 : Form
{
    public static int MouseX = 0;
    public static int MouseY = 0;
    public static DateTime UltimoClick = DateTime.MinValue;

    public Form1()
    {
        InitializeComponent();
    }

    public async void contadorGeral()
    {
        while (iniciado)
        {
            label3.Text = DateTime.Now.ToString();
            label4.Text = $"Tempo até o próximo click: {((DateTime.Now - UltimoClick).Seconds - clickDelay) * -1} segundos";
            await Task.Delay(50);
        }
    }

    public void position(int pid, ExternalMethods.MSLLHOOKSTRUCT mousePosition)
    {
        MouseY = mousePosition.pt.y;
        MouseX = mousePosition.pt.x;

        var process = Process.GetProcessById(pid);
        var processWindowTitle = process.MainWindowTitle;

        var labelText = $@"Título: {process.MainWindowTitle}
Processo: {process.ProcessName}
PID: {process.Id}";


        IntPtr handle = process.MainWindowHandle;

        if (ExternalMethods.GetWindowRect(handle, out ExternalMethods.RECT rect))
        {

            Debug.WriteLine($"X: {rect.Left}");
            Debug.WriteLine($"Y: {rect.Top}");
            Debug.WriteLine($"Width: {rect.Right - rect.Left}");
            Debug.WriteLine($"Height: {rect.Bottom - rect.Top}");
        }
        else
        {
            Debug.WriteLine("Não foi possível obter a posição.");
        }

        labelText += $"X: {mousePosition.pt.x}; Y: {mousePosition.pt.y}";
        label2.Text = labelText;

        this.Location = new Point(rect.Right, rect.Top);
        panel1.Visible = true;
    }


    private void button2_Click(object sender, EventArgs e)
    {
        label2.Text = "Clique no ponto exato...";

        ExternalMethods._proc = ExternalMethods.LowCaptureClickPosition;
        ExternalMethods._hookID = ExternalMethods.SetHook(ExternalMethods._proc);
    }

    bool iniciado = false;
    int clickDelay = 10;
    private void button1_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(textBox1.Text, out clickDelay) || clickDelay == 0)
        {
            MessageBox.Show("Por favor, insira um número inteiro para o atraso entre cliques.");
            return;
        }

        btIniciar.Text = btIniciar.Text == "Iniciar" ? "Parar" : "Iniciar";
        iniciado = !iniciado;
        contadorGeral();


        Task.Run(async () => {
            while (iniciado)
            {
                await Task.Delay(TimeSpan.FromSeconds(clickDelay));
                if (!iniciado)
                {
                    return;
                }
                Debug.WriteLine($"Clicando em X: {MouseX}, Y: {MouseY}");
                ExternalMethods.MoveMouseClickAndReturn();
                UltimoClick = DateTime.Now;
                //ExternalMethods._hookID = ExternalMethods.SetHook(ExternalMethods._proc);
            }
        });
        
    }
}