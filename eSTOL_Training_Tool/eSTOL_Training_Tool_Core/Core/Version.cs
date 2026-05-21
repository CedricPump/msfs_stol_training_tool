using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace STOL_Training_Tool_Core.Core
{
    internal class VersionHelper
    {
        private const string currentVersion = "1.4.33";
        private const string githubApiUrl = "https://api.github.com/repos/CedricPump/msfs_stol_training_tool/releases/latest";
        public static string githubLatestUrl = "https://github.com/CedricPump/msfs_stol_training_tool/releases/latest";
        private const string githubApiUrlFallback = "https://api.github.com/repos/CedricPump/msfs_stol_training_tool/releases/latest";
        public static string githubLatestUrlFallback = "https://github.com/CedricPump/msfs_stol_training_tool/releases/latest";

        public static async Task<string> CheckForUpdateAsync()
        {
            using HttpClient client = new HttpClient();

            // GitHub API requires a user-agent header
            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("STOL_Training_Tool", currentVersion));

            try
            {
                HttpResponseMessage response = await client.GetAsync(githubApiUrl);
                try { response.EnsureSuccessStatusCode(); }
                catch (HttpRequestException e)
                {
                    response = await client.GetAsync(githubApiUrlFallback);
                    response.EnsureSuccessStatusCode();
                }

                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);
                string latestVersion = doc.RootElement.GetProperty("tag_name").GetString();

                if (IsNewerVersion(latestVersion, "v" + currentVersion) && !latestVersion.Contains("-dev"))
                {
                    return latestVersion;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking for update: " + ex.Message);
                return null;
            }
        }

        private static bool IsNewerVersion(string latest, string current)
        {
            Version latestV = ParseVersion(latest);
            Version currentV = ParseVersion(current);

            return latestV > currentV;
        }

        private static Version ParseVersion(string versionString)
        {
            versionString = versionString.TrimStart('v', 'V');
            return Version.TryParse(versionString, out var version) ? version : new Version(0, 0, 0);
        }

        public static string GetVersion() 
        {
            return "v" + currentVersion;
        }
    }
}
