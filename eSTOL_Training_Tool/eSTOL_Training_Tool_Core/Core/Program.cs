using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using eSTOL_Training_Tool_Core.Core;
using eSTOL_Training_Tool_Core.UI;

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
                "│ eSTOL Training Tool │\n" +
                "└─────────────────────┘\n");

            ApplicationConfiguration.Initialize();
            var config = Config.GetInstance();

            string currentDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
            string folderName = Path.GetFileName(currentDir);

            if (folderName != "STOL_Training_Tool")
            {
                RunMigration();
                return;
            }

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


        private static void RunMigration()
        {
            string currentDir =
                AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

            string parent =
                Directory.GetParent(currentDir)!.FullName;

            // New installation location
            string sibling =
                Path.Combine(parent, "STOL_Training_Tool");

            string siblingExe =
                Path.Combine(sibling, "STOL Training Tool.exe");

            // Only migrate from legacy folder
            string currentFolder =
                Path.GetFileName(currentDir);

            if (currentFolder != "eSTOL_Training_Tool")
            {
                return;
            }

            // New structure not present
            if (!Directory.Exists(sibling))
            {
                return;
            }

            // New executable missing
            if (!File.Exists(siblingExe))
            {
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = siblingExe,
                    WorkingDirectory = sibling,
                    UseShellExecute = true
                });

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Migration failed:");
                Console.WriteLine(ex);
            }
        }
    }
}


