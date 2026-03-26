using App2;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.DynamicDependency;
using System;

namespace App2
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                // Inicializa automaticamente o runtime correto
                //Microsoft.WindowsAppSDK.Runtime.Bootstrap.Initialize();
                Bootstrap.Initialize(0); // versão do Windows App SDK

                Application.Start((p) =>
                {
                    var app = new App();
                });
            }
            catch (Exception ex)
            {

                var a = "x";
            }
            
        }
    }
}