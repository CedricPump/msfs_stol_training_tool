using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Reflection;
using System.Text.Json;
using STOL_Training_Tool_Core.Core;
using Newtonsoft.Json;

namespace STOL_Training_Tool_Core.Model
{
    public class Preset
    {
        [JsonProperty("title")]
        public string title = "";

        [JsonProperty("start_lat")]
        public double startLatitude = 0;

        [JsonProperty("start_long")]
        public double startLongitude = 0;

        [JsonProperty("start_alt")]
        public double startAltitude = 0;

        [JsonProperty("start_hdg")]
        public double startHeading = 0;

        public GeoCoordinate getStart()
        {
            return new GeoCoordinate(startLatitude, startLongitude, startAltitude);
        }

        public int getPatternAltitude()
        {
            return (int)(Math.Ceiling(startAltitude * 3.28084 / 100)*100 ) + 500;
        }

        public static List<Preset> ReadPresets(string filePath, string customFilePath)
        {
            List<Preset> presets = new List<Preset>();

            try
            {
                string json;
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("STOL_Training_Tool_Core.presets.json");
                using StreamReader reader = new StreamReader(stream);
                json = reader.ReadToEnd();
                List<Preset> custom = JsonConvert.DeserializeObject<List<Preset>>(json);
                presets.AddRange(custom);

                // read all files of format "{customFilePath}_*.json"
                string[] files = Directory.GetFiles(".", $"{Path.GetFileNameWithoutExtension(customFilePath)}_*.json");
                foreach (string file in files)
                {
                    json = File.ReadAllText(file);
                    custom = JsonConvert.DeserializeObject<List<Preset>>(json);
                    presets.AddRange(custom);
                }

                return presets;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading or deserializing JSON file: {ex.Message}");
                return new List<Preset>();
            }
        }

        public static void SaveCustomPresets(string filePath, Preset preset, string username)
        {
            try
            {
                // trim username for using in filename safely
                username = username.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "").ToLower();
                filePath = $"{Path.GetFileNameWithoutExtension(filePath)}_{username}.json";

                // Serialize the Preset to JSON
                List<Preset> presets = new List<Preset>();
                // load existing presets if the file exists
                if (File.Exists(filePath))
                {
                    string existingJson = File.ReadAllText(filePath);
                    presets = JsonConvert.DeserializeObject<List<Preset>>(existingJson) ?? new List<Preset>();
                }
                // check for duplicates based on title
                if (presets.Exists(p => p.title == preset.title))
                {
                    Console.WriteLine($"Preset with title '{preset.title}' already exists. Skipping save.");
                    return;
                }

                presets.Add(preset);
                string json = JsonConvert.SerializeObject(presets, Formatting.Indented);
                // Write the JSON content to the file
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serializing or writing JSON file: {ex.Message}");
            }
        }

    }
}
