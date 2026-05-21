
using System;
using System.IO;
using System.Text.Json;


namespace STOL_Training_Tool_Core.Core
{
    public class Config
    {
        private const string ConfigFilePath = "config.json";


        public int IdleRefreshInterval { get; set; } = 10000;
        public int RefreshInterval { get; set; } = 50;

        public int PresciseRefreshInterval { get; set; } = 25;

        public uint SimconnectFrames { get; set; } = 2;
        public int TelemetrySendInterval { get; set; } = 3;
        public double GroundspeedThreshold { get; set; } = 0.7;
        public string ResultsExportPath { get; set; } = ".\\export\\eSTOL_Training_Tool.csv";
        public string RecordingExportPath { get; set; } = ".\\export";
        public string ExportPath { get; set; } = ".\\export";
        public string PresetsPath { get; set; } = "presets.json";
        public string CustomPresetsPath { get; set; } = "custom_presets.json";
        public string UserPath { get; set; } = "user.txt";
        public string Unit { get; set; } = "feet";
        public bool debug { get; set; } = false;
        public bool alwaysOnTop { get; set; } = false;
        public bool isSendTelemetry { get; set; } = false;
        public bool isSendResults { get; set; } = false;
        public int uiRefreshIntervall { get; set; } = 1000;
        public bool enableGPXRecodering { get; set; } = true;
        public int transparencyPercent { get; set; } = 0;
        public bool darkModeEnabled { get; set; } = true;
        public bool darkModeSystem { get; set; } = true;
        public bool showTelportConfirmation { get; set; } = true;
        public bool hasPrivacyConfirmed { get; set; } = false;
        public bool simulatePropStrike { get; set; } = true;
        public bool DebugAutoPause { get; set; } = true;
        public bool DebugAutoPauseOnTailTouch { get; set; } = true;

        public uint ResultTextBoxCharacterLimit { get; set; } = 20000;

        public bool PauseOnTeleport { get; set; } = false;
        public bool ForcePauseOnTeleportFromMoving { get; set; } = true;

        public string ConnectionType { get; set; } = "SimConnect"; // or "REST"
        public int ApiPort { get; set; } = 5001;
        public string ApiHost { get; set; } = "127.0.0.1";

        public string influxHost = "https://eu-central-1-1.aws.cloud2.influxdata.com/";
        public string influxBucketResult = "My_eSTOL_Bucket";
        public string influxBucketTelemetry = "My_eSTOL_Bucket";
        public string influxOrg = "steffieth";
        public string influxToken = "";

        public double FuelLevelStandard { get; set; } = 0.5;

        private static Config instance = null;

        public static Config Load()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    instance = JsonSerializer.Deserialize<Config>(json) ?? new Config();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error reading config, using defaults: " + ex.Message);
                    instance = new Config();
                }
            }
            else
            {
                var defaultConfig = new Config();
                defaultConfig.Save(); // Save the default config
                instance = defaultConfig;
            }
            
            return instance;
        }

        public static Config GetInstance() 
        {
            if (instance != null) return instance;
            return Config.Load();
        }  

        public void Save()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            try
            {
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR]: Unable to save config"); 
            }
        }
    }
}
