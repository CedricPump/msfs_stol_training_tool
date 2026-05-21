using System;
using System.IO;
using System.Threading;
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

            CleanupLegacyInstall();

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

        private static void CleanupLegacyInstall()
        {
            try
            {
                string currentDir =
                    AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

                string currentFolder =
                    Path.GetFileName(currentDir);

                if (currentFolder != "STOL_Training_Tool")
                {
                    return;
                }

                string parent =
                    Directory.GetParent(currentDir)!.FullName;

                string oldInstall =
                    Path.Combine(parent, "eSTOL_Training_Tool");

                if (!Directory.Exists(oldInstall))
                {
                    return;
                }

                Thread.Sleep(3000);

                Directory.Delete(oldInstall, true);

                foreach (string file in Directory.GetFiles(currentDir, "eSTOL*.*"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to delete {file}");
                        Console.WriteLine(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Legacy cleanup failed:");
                Console.WriteLine(ex);
            }
        }
    }
}


