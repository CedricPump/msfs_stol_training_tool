using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace STOL_Training_Tool_Core.Core
{
    public class MsfsInstallInfo
    {
        public string Edition { get; set; } = ""; // "Saved", "Microsoft Store" or "Steam"
        public string CommunityPath { get; set; } = null;
        public string InstalledPanelVersion { get; set; } = null;
    }

    /// <summary>
    /// Detects MSFS 2024 installs (Microsoft Store or Steam - 2020 is
    /// intentionally excluded, since the toolbar panel's html_ui/icons/
    /// path convention only works on the 2024 SDK) and installs/updates the
    /// bundled STOL_Training_Panel Community package into one.
    ///
    /// Detection mirrors the sibling VPforce-TelemFFB project's already
    /// field-verified msfs_panel_install.py:
    ///   - Store package family "Microsoft.Limitless_" (2024) under
    ///     %LOCALAPPDATA%\Packages\&lt;name&gt;\LocalCache\UserCfg.opt.
    ///   - Steam: HKCU\Software\Valve\Steam SteamPath (or the default
    ///     C:\Program Files (x86)\Steam), plus every library in
    ///     steamapps\libraryfolders.vdf, looking for steamapps\common\Limitless
    ///     (or "Microsoft Flight Simulator 2024"); UserCfg.opt at
    ///     %APPDATA%\Microsoft Flight Simulator 2024\UserCfg.opt.
    ///   - Either way, the Community folder is "&lt;InstalledPackagesPath from
    ///     UserCfg.opt&gt;\Community" - that path is the last non-empty line
    ///     of UserCfg.opt - falling back to the default Packages\Community
    ///     folder if UserCfg.opt can't be parsed.
    ///   - A folder the user picked by hand is saved to line 2 of user.txt
    ///     (line 1 is the username) and takes priority over detection.
    /// </summary>
    public static class MsfsPanelInstall
    {
        private const string PanelFolderName = "STOL_Training_Panel";
        private const string DefaultSteamPath = @"C:\Program Files (x86)\Steam";
        private static readonly string[] SteamGameFolderNames = { "Limitless", "Microsoft Flight Simulator 2024" };

        private static string SteamUserDataDir => Path.Combine(
            Environment.GetEnvironmentVariable("APPDATA") ?? "", "Microsoft Flight Simulator 2024");

        private static readonly Regex InstalledPackagesPathRe =
            new Regex("InstalledPackagesPath\\s+\"([^\"]+)\"", RegexOptions.Compiled);
        private static readonly Regex VdfPathRe =
            new Regex("\"path\"\\s+\"([^\"]+)\"", RegexOptions.Compiled);

        public static string BundledPanelDir =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PanelFolderName);

        public static string GetBundledPanelVersion()
        {
            return ReadManifestVersion(Path.Combine(BundledPanelDir, "manifest.json"));
        }

        private static string ReadManifestVersion(string manifestPath)
        {
            try
            {
                if (!File.Exists(manifestPath)) return null;
                using var doc = JsonDocument.Parse(File.ReadAllText(manifestPath));
                if (doc.RootElement.TryGetProperty("package_version", out var v))
                {
                    return v.GetString();
                }
            }
            catch { }
            return null;
        }

        private static string CommunityPathFromUserCfg(string usercfgPath)
        {
            try
            {
                var lines = File.ReadAllLines(usercfgPath)
                    .Select(l => l.Trim())
                    .Where(l => l.Length > 0)
                    .ToList();
                if (lines.Count == 0) return null;

                var m = InstalledPackagesPathRe.Match(lines[lines.Count - 1]);
                if (!m.Success) return null;

                return Path.Combine(m.Groups[1].Value, "Community");
            }
            catch
            {
                return null;
            }
        }

        private static MsfsInstallInfo FindStoreInstall()
        {
            string packagesDir = Path.Combine(
                Environment.GetEnvironmentVariable("LOCALAPPDATA") ?? "", "Packages");
            if (!Directory.Exists(packagesDir)) return null;

            string match;
            try
            {
                match = Directory.GetDirectories(packagesDir)
                    .Select(Path.GetFileName)
                    .FirstOrDefault(n => n.StartsWith("Microsoft.Limitless_", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
            if (match == null) return null;

            string localCache = Path.Combine(packagesDir, match, "LocalCache");
            string usercfg = Path.Combine(localCache, "UserCfg.opt");
            if (!File.Exists(usercfg)) return null;

            return new MsfsInstallInfo
            {
                Edition = "Microsoft Store",
                CommunityPath = CommunityPathFromUserCfg(usercfg)
                    ?? ExistingDir(Path.Combine(localCache, "Packages", "Community"))
            };
        }

        private static List<string> SteamLibraryPaths()
        {
            var libraries = new List<string>();
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
                string steamPath = key?.GetValue("SteamPath") as string;
                if (string.IsNullOrEmpty(steamPath)) steamPath = DefaultSteamPath;
                libraries.Add(steamPath);
                if (!string.Equals(Path.GetFullPath(steamPath), DefaultSteamPath, StringComparison.OrdinalIgnoreCase))
                {
                    libraries.Add(DefaultSteamPath);
                }

                string vdfPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
                if (File.Exists(vdfPath))
                {
                    string content = File.ReadAllText(vdfPath);
                    foreach (Match m in VdfPathRe.Matches(content))
                    {
                        libraries.Add(m.Groups[1].Value.Replace("\\\\", "\\"));
                    }
                }
            }
            catch { }
            return libraries;
        }

        private static MsfsInstallInfo FindSteamInstall()
        {
            bool gameFound = SteamLibraryPaths().Any(lib => SteamGameFolderNames.Any(name =>
                Directory.Exists(Path.Combine(lib, "steamapps", "common", name))));

            string usercfg = Path.Combine(SteamUserDataDir, "UserCfg.opt");
            string communityPath = (File.Exists(usercfg) ? CommunityPathFromUserCfg(usercfg) : null)
                ?? ExistingDir(Path.Combine(SteamUserDataDir, "Packages", "Community"));

            // The default Community folder existing is good enough evidence
            // even if the game itself lives in a library we didn't look in.
            if (!gameFound && communityPath == null) return null;

            return new MsfsInstallInfo
            {
                Edition = "Steam",
                CommunityPath = communityPath
            };
        }

        private static string ExistingDir(string path)
        {
            return !string.IsNullOrEmpty(path) && Directory.Exists(path) ? path : null;
        }

        /// <summary>A good starting folder for the browse dialog.</summary>
        public static string SuggestedBrowseDir()
        {
            return ExistingDir(Path.Combine(SteamUserDataDir, "Packages"))
                ?? ExistingDir(SteamUserDataDir)
                ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>If the user picked the parent of a Community folder (e.g.
        /// Packages\), use the Community folder inside it.</summary>
        public static string NormalizeCommunitySelection(string selected)
        {
            string name = Path.GetFileName(selected.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.Equals(name, "Community", StringComparison.OrdinalIgnoreCase))
            {
                return selected;
            }
            return ExistingDir(Path.Combine(selected, "Community")) ?? selected;
        }

        /// <summary>The user-chosen Community folder from line 2 of user.txt,
        /// or null if none is saved.</summary>
        public static string LoadSavedCommunityPath()
        {
            try
            {
                string userPath = Config.GetInstance().UserPath;
                if (!File.Exists(userPath)) return null;
                string[] lines = File.ReadAllLines(userPath);
                if (lines.Length < 2) return null;
                string path = lines[1].Trim();
                return path.Length > 0 ? path : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Saves the Community folder to line 2 of user.txt, keeping
        /// the username on line 1.</summary>
        public static void SaveCommunityPath(string communityPath)
        {
            string userPath = Config.GetInstance().UserPath;
            var lines = File.Exists(userPath) ? File.ReadAllLines(userPath).ToList() : new List<string>();
            while (lines.Count < 2) lines.Add("");
            lines[1] = communityPath;
            File.WriteAllLines(userPath, lines);
        }

        /// <summary>The saved Community folder if it still exists, else the
        /// first detected MSFS 2024 install (Store preferred over Steam if,
        /// unusually, both are present), or null if none found.
        /// MSFS 2020 is never returned - see class remarks.</summary>
        public static MsfsInstallInfo FindMsfs2024Install()
        {
            string saved = ExistingDir(LoadSavedCommunityPath());
            MsfsInstallInfo info = saved != null
                ? new MsfsInstallInfo { Edition = "Saved", CommunityPath = saved }
                : FindStoreInstall() ?? FindSteamInstall();
            if (info != null && info.CommunityPath != null)
            {
                info.InstalledPanelVersion = ReadManifestVersion(
                    Path.Combine(info.CommunityPath, PanelFolderName, "manifest.json"));
            }
            return info;
        }

        /// <summary>Copies the bundled panel into &lt;communityPath&gt;\STOL_Training_Panel,
        /// overwriting an existing install in place (used for both first
        /// install and updates). Excludes the Build\ subfolder, which is only
        /// needed to compile the .spb, not to run it.</summary>
        public static void InstallPanel(string communityPath)
        {
            string src = BundledPanelDir;
            if (!Directory.Exists(src))
            {
                throw new DirectoryNotFoundException($"Bundled panel not found at {src}");
            }

            string dst = Path.Combine(communityPath, PanelFolderName);
            Directory.CreateDirectory(communityPath);
            CopyDirectory(src, dst);
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string name = Path.GetFileName(dir);
                if (string.Equals(name, "Build", StringComparison.OrdinalIgnoreCase)) continue;
                CopyDirectory(dir, Path.Combine(destDir, name));
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(destDir, Path.GetFileName(file)), overwrite: true);
            }
        }
    }
}
