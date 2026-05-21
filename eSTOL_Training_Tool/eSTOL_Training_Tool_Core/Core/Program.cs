using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using STOL_Training_Tool_Core.Core;
using STOL_Training_Tool_Core.UI;

namespace Bombathlon
{
    static class Program
    {
        static void Main(string[] args)
        {
            var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.log");
            var fileStream = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            var writer = new StreamWriter(fileStream) { AutoFlush = true };
            Console.SetOut(writer);
            Console.SetError(writer);

            // Influx.GetInstance().deletAll();
            Console.WriteLine(
                "┌─────────────────────┐\n" +
                "│ STOL Training Tool │\n" +
                "└─────────────────────┘\n");

            ApplicationConfiguration.Initialize();
            var config = Config.GetInstance();

            Controller controller = new Controller();
            controller.Init();
            Task controllerTask = Task.Run(() =>
            {
                controller.Run(); // Run the loop in the background
            });

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (config.darkModeEnabled)
            {
                if (config.darkModeSystem)
                {
                    Application.SetColorMode(SystemColorMode.System);
                }
                else
                {
                    Application.SetColorMode(SystemColorMode.Dark);
                }
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            var form = new FormUI(controller);
            controller.SetUI(form);

            Application.Run(form);
        }
    }
}


