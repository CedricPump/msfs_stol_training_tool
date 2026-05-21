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

            string currentFolder =
                Path.GetFileName(currentDir);

            if (currentFolder != "eSTOL_Training_Tool")
            {
                return;
            }

            string parent =
                Directory.GetParent(currentDir)!.FullName;

            string newInstall =
                Path.Combine(parent, "STOL_Training_Tool");

            string newExe =
                Path.Combine(newInstall, "STOL Training Tool.exe");

            try
            {
                if (!Directory.Exists(newInstall))
                {
                    Directory.CreateDirectory(newInstall);

                    foreach (string sourcePath in Directory.GetFiles(
                                 currentDir,
                                 "*",
                                 SearchOption.AllDirectories))
                    {
                        string relative =
                            Path.GetRelativePath(currentDir, sourcePath);

                        string destination =
                            Path.Combine(newInstall, relative);

                        string? destinationDir =
                            Path.GetDirectoryName(destination);

                        if (!string.IsNullOrEmpty(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                        }

                        try
                        {
                            File.Copy(sourcePath, destination, true);
                            File.Delete(Path.Combine(destinationDir, "eSTOL Training Tool.exe"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to copy: {sourcePath}");
                            Console.WriteLine(ex);
                        }
                    }

                }
                if (File.Exists(newExe))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = newExe,
                        WorkingDirectory = newInstall,
                        UseShellExecute = true
                    });

                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Migration failed:");
                Console.WriteLine(ex);
            }
        }
    }
}


