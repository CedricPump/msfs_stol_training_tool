using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using STOL_Training_Tool.Model;
using STOL_Training_Tool_Core.Core;
using STOL_Training_Tool_Core.UI;

namespace STOL_Training_Tool_Core.Model
{
    public static class PlaneConfigsService
    {
        private static Dictionary<string, PlaneConfig> planeConfigByKey = new();
        private static List<PlaneConfig> planeConfigs = new();

        public static int LoadPlaneConfigs()
        {
            var names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("STOL_Training_Tool_Core.PlanesConfig.json");

            using StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            planeConfigs = JsonSerializer.Deserialize<List<PlaneConfig>>(json) ?? new();

            planeConfigByKey.Clear();
            foreach (PlaneConfig config in planeConfigs)
            {
                if (!string.IsNullOrWhiteSpace(config.Key))
                {
                    planeConfigByKey[config.Key] = config;
                }
            }
            return planeConfigs.Count;

        }

        public static bool HasPlaneConfig(string key)
        {
            return planeConfigByKey.ContainsKey(key);
        }

        public static bool HasPlaneConfig(Ident ident)
        {
            return GetPlaneConfigKey(ident) != null;
        }

        public static string GetPlaneConfigKey(Ident ident)
        {
            // concat model, type and title to get the key, e.g. "ATR|42-600S|ATR 42-600S White"
            string key = ident.ToString();
            // loop through the plane configs and find the first one that matches the key using the regex pattern, e.g. "ATR\|42-600S\|.*"

            string foundKey = "DEFAULT";

            UILogger.LogDebug($"[DEBUG] checking {planeConfigs.Count} entries for key: {key}");
     
            foreach (var config in planeConfigs) 
            {
 
                UILogger.LogDebug($"[DEBUG] matching: key with regex: \\{config.Regex}\\");
                
                // check key regex pattern, if it matches the ident key, return the config key
                if (!string.IsNullOrWhiteSpace(config.Regex) && System.Text.RegularExpressions.Regex.IsMatch(key, config.Regex))
                {
                    foundKey = config.Key;
                    UILogger.LogDebug($"[DEBUG] match: {config.DisplayName}!");
                    break;
                }
            }

            return foundKey;
        }

        public static PlaneConfig GetPlaneConfig(string key)
        {
            if (key == null) return GetDefaultConfig();
           
            if (!planeConfigByKey.TryGetValue(key, out PlaneConfig planeConfig))
            {
                planeConfig = GetDefaultConfig();
            }
            return planeConfig;
        }


        private static PlaneConfig GetDefaultConfig() 
        {
            PlaneConfig planeConfig;
            // fallback to DEFAULT
            if (planeConfigByKey.TryGetValue("DEFAULT", out var defaultConfig))
            {
                planeConfig = defaultConfig;
            }
            else
            {
                planeConfig = new PlaneConfig { Key = "DEFAULT", GearOffset = -0.45f }; // hard fallback
            }
            return planeConfig;
        }

        public static float GetGearOffset(string aircraftType)
        {
            return GetPlaneConfig(aircraftType).GearOffset;
        }
    }

    public class PlaneConfig
    {
        public string Key { get; set; } = "";

        public string Regex { get; set; } = "";
        public float GearOffset { get; set; } = 0.0f;
        // add other properties as needed, e.g.
        public string DisplayName { get; set; } = "";
        public bool IsTaildragger { get; set; } = true;
        public string Class { get; set; } = "";
        // Colision Point Index
        public int CollisionNoseIndex { get; set; } = -1;
        public int CollisionPropIndex { get; set; } = -1;
        public uint CollisionWheelLeftIndex { get; set; } = 1;
        public uint CollisionWheelNoseTailIndex { get; set; } = 0;
        public uint CollisionWheelRightIndex { get; set; } = 2;
        public uint CollisionWheelWingtipLIndex { get; set; } = 3;
        public uint CollisionWheelWingtipRIndex { get; set; } = 4;
        public uint PropStrikeThreshold { get; set; } = 30; // deg
        public float MaxGForce { get; set; } = 2.0f; // G
        public int MaxVSpeed { get; set; } = -1000; // ft/min
    }
}
