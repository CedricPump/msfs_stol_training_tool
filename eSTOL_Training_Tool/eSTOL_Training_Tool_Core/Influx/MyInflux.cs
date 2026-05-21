using System;
using STOL_Training_Tool;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using STOL_Training_Tool.Model;
using STOL_Training_Tool_Core.Core;


namespace STOL_Training_Tool_Core.Influx
{
    public class MyInflux
    {
        private string influxHost = "https://eu-central-1-1.aws.cloud2.influxdata.com/";
        private string bucketResult = "My_eSTOL_Bucket";
        private string bucketTelemetry = "My_eSTOL_Bucket";
        private string org = "steffieth";
        private Core.Config config;

        InfluxDBClient influxDBClient;


        private static MyInflux instance;

        public static MyInflux GetInstance()
        {
            if (instance == null)
                instance = new MyInflux();

            return instance;
        }

        private MyInflux()
        {
            config = Core.Config.GetInstance();
            this.influxHost = config.influxHost;
            this.bucketResult = config.influxBucketResult;
            this.bucketTelemetry = config.influxBucketTelemetry;
            this.org = config.influxOrg;
            string influxToken = config.influxToken;

            if (influxToken == null || influxToken == "")
            {
                influxToken = InfluxToken.Token;
            }

            influxDBClient = new InfluxDBClient(influxHost, influxToken);
        }

        public async void sendData(STOLResult stolResult)
        {
            var point = PointData.Measurement("stol_results")
                .Tag("User", stolResult.User)
                .Tag("planeType", stolResult.planeType)
                .Tag("preset", stolResult.preset == null ? "" : stolResult.preset.title)
                .Tag("SessionKey", stolResult.sessionKey)
                .Tag("UUID", stolResult.UUID)
                .Field("Takeoffdist", stolResult.Takeoffdist)
                .Field("Touchdowndist", stolResult.Touchdowndist)
                .Field("Stoppingdist", stolResult.Stoppingdist)
                .Field("Landingdist", stolResult.Landingdist)
                .Field("MaxSpin", stolResult.maxSpin)
                .Field("MaxPitch", stolResult.minPitch)
                .Field("MaxBank", stolResult.maxBank)
                .Field("GrndSpeed", stolResult.GrndSpeed)
                .Field("VSpeed", stolResult.VSpeed)
                .Field("GForce", stolResult.GForce)
                .Field("Score", stolResult.Score)
                .Field("PatternTime", stolResult.PatternTime.TotalSeconds)
                .Field("InitHash", stolResult.InitHash)
                .Field("Pressure", stolResult.ambientPressure)
                .Field("Temperature", stolResult.ambientTemperature)
                .Field("FieldElevation", stolResult.fieldElevation)
                .Field("LandingFuelPercent", stolResult.landingFuelPercent)
                .Field("TakeoffWindSpeed", stolResult.takeoffWindSpeed)
                .Field("TakeoffWindDirection", stolResult.takeoffWindDirection)
                .Field("LandingWindSpeed", stolResult.landingWindSpeed)
                .Field("LandingWindDirection", stolResult.landingWindDirection)
                .Timestamp(stolResult.time, WritePrecision.Ns);

            try { 
                var writeApi = influxDBClient.GetWriteApiAsync();
                await writeApi.WritePointAsync(point, bucketResult, org);
        }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending telemetry: {ex.Message}");
            }
}

        public async void sendTelemetry(string username, Plane plane, STOLData stol)
        {
            Telemetrie telemetrie = plane.GetTelemetrie();
            AircraftState state = plane.GetState();

            string preset = "";
            if (stol != null && stol.preset != null)
            {
                preset = stol.preset.title;
            }

            var point = PointData.Measurement("stol_telemetry")
                .Tag("User", username)
                .Tag("Model", plane.Model)
                .Tag("VersionTag", VersionHelper.GetVersion())
                .Tag("SelectedPreset", preset)
                .Tag("SessionKey", stol.sessionKey)
                .Tag("UUID", stol.UUID)
                .Field("Heading", telemetrie.Heading)
                .Field("Latitude", telemetrie.Position.Latitude)
                .Field("Longitude", telemetrie.Position.Longitude)
                .Field("Altitude", telemetrie.Altitude)
                .Field("AltAGL", telemetrie.AltitudeAGL)
                .Field("GroundSpeed", telemetrie.GroundSpeed)
                .Field("Fuel", state.Fuel)
                .Field("FuelPercent", state.FuelPercent)
                .Field("UnlimitedFuel", state.FuelUnlimited ? 1.0 : 0.0)
                .Field("Weight", state.Weight)
                .Field("PilotWeight", state.PilotWeight)
                .Field("MaxWeightPercent", state.MaxWeightPercent)
                .Field("ParkingBrake", state.ParkingBrake ? 1.0 : 0.0)
                .Field("Antistall", plane.Antistall)
                .Field("PropRpm", plane.PropRPM)
                .Timestamp(DateTime.Now, WritePrecision.Ns);

            try
            {
                var writeApi = influxDBClient.GetWriteApiAsync();
                await writeApi.WritePointAsync(point, bucketTelemetry, org);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending telemetry: {ex.Message}");
            }

        }

        public async void sendEvent(string username, STOLData stol, Plane plane, string eventType, string value = "")
        {

            Telemetrie telemetrie = plane.GetTelemetrie();
            AircraftState state = plane.GetState();
            var point = PointData.Measurement("stol_event")
                .Tag("EventType", eventType)
                .Tag("User", username)
                .Tag("SessionKey", stol.sessionKey)
                .Tag("UUID", stol.UUID)
                .Field("Value", value)
                .Field("Latitude", telemetrie.Position.Latitude)
                .Field("Longitude", telemetrie.Position.Longitude)
                .Field("Altitude", telemetrie.Altitude)
                .Field("AltAGL", telemetrie.AltitudeAGL)
                .Timestamp(DateTime.Now, WritePrecision.Ns);
            try
            {
                var writeApi = influxDBClient.GetWriteApiAsync();
                await writeApi.WritePointAsync(point, bucketTelemetry, org);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending event: {ex.Message}");
            }
        }

        public async void deletAll()
        {
            var start = DateTime.MinValue;
            var stop = DateTime.UtcNow;

            try
            {
                // Perform the delete operation
                var deleteApi = influxDBClient.GetDeleteApi();
                await deleteApi.Delete(start, stop, "", bucketResult, org);
                await deleteApi.Delete(start, stop, "", bucketTelemetry, org);

                Console.WriteLine("All data from the bucket has been cleared.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting data: {ex.Message}");
            }
        }


    }
}
