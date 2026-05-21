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

            if (folderName == "eSTOL_Training_Tool")
            {
                RunMigration(currentDir);
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

        private static void RunMigration(string oldPath)
        {
            string parentDir = Directory.GetParent(oldPath)!.FullName;
            string newPath = Path.Combine(parentDir, "STOL_Training_Tool");

            string tempScript = Path.Combine(Path.GetTempPath(), "stol_migrate.ps1");

            string script = @$"
param(
    [string]$oldPath,
    [string]$newPath
)

Start-Sleep -Seconds 2

if (Test-Path $newPath) {{
    Remove-Item $newPath -Recurse -Force
}}

Rename-Item $oldPath ""STOL_Training_Tool""

$exe = Join-Path $newPath ""STOL Training Tool.exe""

Start-Process $exe
";

            File.WriteAllText(tempScript, script);

            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments =
                    $"-ExecutionPolicy Bypass -File \"{tempScript}\" " +
                    $"\"{oldPath}\" \"{newPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            Environment.Exit(0);
        }
    }
}


