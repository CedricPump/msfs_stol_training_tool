using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using STOL_Training_Tool;
using STOL_Training_Tool_Core.GPX;
using STOL_Training_Tool_Core.Influx;
using STOL_Training_Tool_Core.Model;
using STOL_Training_Tool_Core.UI;
using Microsoft.FlightSimulator.SimConnect;

namespace STOL_Training_Tool_Core.Core
{
    enum CycleState
    {
        Park,
        Taxi,
        Takeoff,
        Climbout,
        Fly,
        Land,
        Rollout,
        Hold,
        Unknown
    }

    public class Controller
    {
        public Plane plane;
        Config config;
        CycleState cycleState = CycleState.Unknown;
        public STOLData stol = new STOLData();
        STOLData lastStol = new STOLData();
        Telemetrie lastTelemetrie;
        GPXRecorder GPXRecorder = new GPXRecorder();
        bool openWorldMode = true;
        List<Preset> presets = new();
        Preset preset;
        public string user = "";
        MyInflux influx = MyInflux.GetInstance();
        FormUI? form;
        string unit;
        DateTime lastTelemetrieTime = DateTime.MinValue;
        DateTime lastUIResfresh = DateTime.MinValue;
        double AGLonGroundThreshold = 0.3; // ft
        double fieldLength = 600; // ft
        bool debug = false;
        double flagsAngleTreshold = 1; // deg
        int BankAngleLimit = 75;
        Telemetrie telemetrieLocked = null;
        public bool isPaused = false;

        public Controller()
        {
            this.config = Config.Load();
            
            this.unit = config.Unit;
            
            if(config.ConnectionType == "SimConnect") 
            {
                AppendResult("[INFO]: Using SimConnect API");
                plane = new AircraftSimConnect(OnPlaneEventCallback);
            }
            else if(config.ConnectionType == "REST")
            {
                AppendResult("[INFO]: Using Telemetry InGamePanel API");
                plane = new AircraftApi(OnPlaneEventCallback);
            }


            var exportDir = Path.GetDirectoryName(config.ResultsExportPath);
            // fix wrong export file from 1.3.1
            if (File.Exists(exportDir))
            {
                File.Delete(exportDir);
            }
            // add export dir#
            if (!Directory.Exists(exportDir))
            {
                Directory.CreateDirectory(exportDir);
            }

            // init export file
            if (!File.Exists(config.ResultsExportPath))
            {
                // Create the file and write the header
                using (StreamWriter writer = new StreamWriter(config.ResultsExportPath))
                {
                    writer.WriteLine(STOLResult.getCSVHeader());
                }
            }
            if (config.debug) AppendResult("[DEBUG]: exporting to " + config.ExportPath);
        }

        public void SetUI(FormUI form) 
        {
            this.form = form;
            this.form.setPresets(presets.Select(p => p.title).ToArray());
        }

        public void Init()
        {
            int configs = PlaneConfigsService.LoadPlaneConfigs();
            AppendResult($"Loaded {configs} plane configs");

            // Update once to trigger connect to sim
            plane.Update();

            //plane.SpawnObject("Cone",plane.Latitude,plane.Longitude,plane.Altitude);

            LoadUser();

            CheckForUpdateStartup();

            // Load presets
            reloadPresets();

            openWorldMode = true;
            return;
        }

        private void LoadUser()
        {
            if (!File.Exists(config.UserPath))
            {
                // Disclaimer
                string disclaimer = "Disclaimer:\n"
                    + "This tool is a work in progress and is primarily intended for training purposes.\n"
                    + "It may contain bugs or incomplete features, and some values may not reflect perfect accuracy.\n"
                    + "Final competition results are determined solely by the official judge using all tools of their discretion.\n"
                    + "Please make sure to record your flight for any required score verification or challenges.\n\n";
                Console.Write("\n" + disclaimer);

                MessageBox.Show(disclaimer);

                using (var userForm = new FormFirstUser())
                {
                    if (userForm.ShowDialog() == DialogResult.OK)
                    {
                        string name = userForm.Username?.Trim() ?? "";

                        using (StreamWriter writer = new StreamWriter(config.UserPath))
                        {
                            writer.WriteLine(name);
                            user = name;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No username entered. Upload will be skipped.", "Info");
                        user = "";
                    }
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(config.UserPath))
                {
                    user = reader.ReadLine(); // Read the username
                }
            }
            stol.user = user;
        }

        private async void CheckForUpdateStartup()
        {
            string? result = await VersionHelper.CheckForUpdateAsync();
            if (result != null)
            {
                using var dialog = new UpdateDialog(result);
                dialog.ShowDialog();

                if (dialog.shouldUpdate)
                {
                    await PerformUpdate(result);
                }
            }
        }

        public async Task CheckForUpdateManual()
        {
            string? result = await VersionHelper.CheckForUpdateAsync();

            using var dialog = new FormCheckUpdate(result != null ? result : "", VersionHelper.GetVersion(), result != null);
            dialog.ShowDialog();

            if (dialog.shouldUpdate)
            {
                await PerformUpdate(dialog.updateVersion);
            }
        }

        private async Task PerformUpdate(string version)
        {
            try
            {
                string zipFileName = $"eSTOL_Training_Tool_portable_{version}.zip";
                string zipUrl = $"https://github.com/CedricPump/msfs_stol_training_tool/releases/download/{version}/{zipFileName}";
                string tempZipPath = Path.Combine(Path.GetTempPath(), "update.zip");
                string bootstrapperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "update.ps1");

                using HttpClient client = new HttpClient();
                using var stream = await client.GetStreamAsync(zipUrl);
                using var fs = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fs);

                AppendResult($"Saved update to: {tempZipPath}");

                string arguments = $"-ExecutionPolicy Bypass -File \"{bootstrapperPath}\" \"{tempZipPath}\" \"{AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)}\"";

                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = arguments,
                    UseShellExecute = false
                });



                Application.Exit();
            } catch 
            {
                AppendResult("Update failed");
            }
        }


        public string createPreset() 
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            return $"Preset JSON:\r\n\r\n{{\"title\": \"YOUR TITLE\", \"start_lat\": {plane.GetTelemetrie().Position.Latitude.ToString("0.000000", nfi)}, \"start_long\": {plane.GetTelemetrie().Position.Longitude.ToString("0.000000", nfi)}, \"start_alt\": {plane.GetTelemetrie().Position.Altitude.ToString("0", nfi)}, \"start_hdg\": {plane.GetTelemetrie().Heading.ToString("0", nfi)}}}\r\n\r\ncopy and insert to presets.json";
        }

        public Preset createPresetFromCurrent() 
        {
            Preset preset = new Preset();
            preset.startLatitude = plane.GetTelemetrie().Position.Latitude;
            preset.startLongitude = plane.GetTelemetrie().Position.Longitude;
            preset.startAltitude = plane.GetTelemetrie().Position.Altitude;
            preset.startHeading = plane.GetTelemetrie().Heading;
            preset.title = "";
            return preset;
        }

        public void SetUser(string username)
        {
            using (StreamWriter writer = new StreamWriter(config.UserPath))
            {
                writer.WriteLine(username);
                user = username;
            }
            stol.user = username;
        }

        public void SetSession(string sessionKey)
        {
            stol.sessionKey = sessionKey;
        }

        public void setUnit(string unit) 
        {
            this.unit = unit;
            if(stol != null && stol.StopPosition !=  null) 
            {
                AppendResult(stol.GetResult(this.unit).getConsoleString());
            }
        }

        public void ReinitPlaneType() 
        {

            this.plane.ResetConfig();
            this.stol.planeName = plane.GetDisplayName();
            this.stol.planeIdentStr = plane.GetIdent().ToString();
            this.stol.planeKey = plane.ConfigKey;
            string hasConfigText = plane.HasPlaneConfig() ? "config found" : "no config";
            this.AppendResult($"Plane Changed: {this.stol.planeName} {hasConfigText}");
            
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    plane.Update();
                    if(!plane.isInit) { 
                        plane.Update();
                        // if(form != null) AppendResult("Test " + DateTime.Now.ToFileTimeUtc());
                        if(form != null) form.setAligned("Sim not connected", Color.Red);
                        System.Threading.Thread.Sleep(330);
                        continue;
                    };

                    if (plane.Title == null)
                    {
                        if (form != null) form.setAligned("Sim not initialized", Color.DarkOrange);
                        plane.Update();
                        System.Threading.Thread.Sleep(330);
                        continue;
                    };


                    if(plane.GetIdent().ToString() != this.stol.planeIdentStr) 
                    {
                        ReinitPlaneType();
                    }

                    if (plane.IsSimConnected && plane.isInit) // && !plane.SimDisabled)
                    {
                        Telemetrie telemetrie = plane.GetTelemetrie();

                        // if (config.debug) AppendResult($"Wind: {Math.Round(plane.getWindTotal()):F1} knt @ {Math.Round(plane.GetWindDirectionRelativeRL()):F0}° rel");

                        // send telemetry and write debug
                        if (config.isSendTelemetry && stol.user != null && stol.user != "")
                        {
                            if (DateTime.Now - lastTelemetrieTime > TimeSpan.FromSeconds(config.TelemetrySendInterval))
                            {
                                lastTelemetrieTime = DateTime.Now;
                                try
                                {
                                    influx.sendTelemetry(stol.user, plane, stol);
                                }
                                catch (Exception ex)
                                {
                                    AppendResult("[ERROR]: Unable to send telemetry - check update");
                                }

                                if (config.debug && stol.IsInit()) AppendResult($"DEBUG: distance to line: {stol.GetDistanceTo(telemetrie.Position) * 3.28084:F0}ft AGL: {telemetrie.AltitudeAGL:F0}");

                                if (config.debug && stol.IsInit())
                                {
                                    (double, double) xy = stol.GetDistanceAndOffsetTo(telemetrie.Position);
                                    AppendResult($"DEBUG: offset: (x={Math.Round(xy.Item1)}m, y={Math.Round(xy.Item2)}m");
                                }
                            }
                        }

                        if (stol.IsInit() && config.debug)
                        {
                            if (DateTime.Now - lastUIResfresh > TimeSpan.FromSeconds(config.uiRefreshIntervall))
                            {
                                this.form.setPlanePos(stol.InitialPosition, (double)stol.InitialHeading, telemetrie.Position);
                            }
                        }

                        if (stol.IsInit() &&  plane.isInit && form != null)
                        {
                            if (DateTime.Now - lastUIResfresh > TimeSpan.FromMilliseconds(config.uiRefreshIntervall))
                            {
                                form.setWind(plane.getRelWindDir(), plane.getWindTotal());
                                form.setCollisionWheels();

                                lastUIResfresh = DateTime.Now;
                            }

                            // kill engine
                            if (plane.IsPropstrike() && config.simulatePropStrike)
                            {
                                if(config.debug && config.DebugAutoPause)
                                {
                                   this.PauseNoPopup(plane, $"Propstrike detected: {plane.pitch}°");
                                }

                                plane.setDoubleValue("GENERAL ENG COMBUSTION:0", 0);
                                plane.setDoubleValue("GENERAL ENG COMBUSTION:1", 0);
                                plane.setDoubleValue("GENERAL ENG COMBUSTION:2", 0);
                            }

                            if ( stol.IsInit() && plane.isInit)
                            {
                                if(plane.WingtipOnGround()) 
                                {

                                    if (!stol.hasDeviation("WingStrike"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("WingStrike", 1, 3));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "WINGSTRIKE", "true");
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }

                                if(plane.IsPropstrike()) 
                                {
                                    if (!stol.hasDeviation("PropStrike"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("PropStrike", 1, 3));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "PROPSTRIKE", "true");
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }


                                if (plane.IsSlew)
                                {
                                    if (!stol.hasDeviation("Slew"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("Slew", 1, 0));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "SLEW", "true");
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }

                                if (plane.IsVNEOverspeed)
                                {
                                    if (!stol.hasDeviation("OverspeedVNE"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("OverspeedVNE", plane.Airspeed, 2));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "OVERSPEED_VNE", ((double)plane.Airspeed).ToString("0"));
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }

                                if (plane.IsFlapsOverspeed)
                                {
                                    if (!stol.hasDeviation("OverspeedFlaps"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("OverspeedFlaps", plane.Airspeed, 2));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "OVERSPEED_FLAPS", ((double)plane.Airspeed).ToString("0"));
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }

                                if (plane.VerticalSpeed > 3000)
                                {
                                    if (!stol.hasDeviation("HighClimbRate"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("HighClimbRate", plane.VerticalSpeed));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "HIGH_CLIMB_RATE", ((double)plane.VerticalSpeed).ToString("0"));
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }

                                if (Math.Abs(plane.bank) > BankAngleLimit)
                                {
                                    if (!stol.hasDeviation("HightBankAngle"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("HightBankAngle", plane.bank));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "HIGH_BANK_ANGLE", ((double)plane.bank).ToString("0.0"));
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }

                                if (plane.Antistall != 0 || plane.Autotrim || plane.AICtrl )
                                {
                                    if (!stol.hasDeviation("AssistsAI"))
                                    {
                                        stol.deviations.Add(new STOLDeviation("AssistsAI", 1, 3));
                                        try
                                        {
                                            // send event
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "Assits_AI", "true");
                                        }
                                        catch
                                        {
                                            AppendResult("[ERROR]: Unable to send event - check update");
                                        }
                                    }
                                }
                            }


                            switch (cycleState)
                            {
                                case CycleState.Unknown:
                                    if (plane.IsStopped())
                                    {
                                        setState(CycleState.Hold);
                                    }
                                    else if (plane.GroundSpeed > config.GroundspeedThreshold && !plane.MainGearOnGround())
                                    {
                                        // hotstart in air
                                        setState(CycleState.Fly);
                                        stol.TakeoffPosition = stol.InitialPosition;
                                        stol.TakeoffTime = DateTime.Now;
                                        stol.deviations.Add(new STOLDeviation("HotstartInAir", 1, 0));
                                        if (this.form != null) this.form.setAligned("", SystemColors.Control);
                                    }
                                    break;
                                case CycleState.Hold:
                                    {
                                        if (IsStolInit())
                                        {
                                            (double yOffset, double xOffset) = GeoUtils.GetDistanceAlongAxis(stol.InitialPosition, plane.getPositionWithGearOffset(), (double)stol.InitialHeading);
                                            double spin = GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                            (double angleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);;

                                            if (!(spin > angleR + flagsAngleTreshold || spin < angleL - flagsAngleTreshold) && yOffset > -1.2 && yOffset < 0 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned($"aligned ({Math.Round(stol.GetDistanceTo(telemetrie.Position) * 3.28084):F0} ft)", System.Drawing.Color.LightGreen);
                                            }
                                            else if (Math.Abs(spin) < 45 && yOffset > -1.2 && yOffset < 1 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned($"aligned (bad heading,{Math.Round(stol.GetDistanceTo(telemetrie.Position) * 3.28084):F0} ft)", System.Drawing.Color.LightGreen);
                                            }
                                            else if (Math.Abs(spin) < 90 && yOffset > -180 && yOffset < 1 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned($"on lineup ({Math.Round(stol.GetDistanceTo(telemetrie.Position) * 3.28084):F0} ft)", System.Drawing.Color.LightYellow);
                                            }
                                            else if (Math.Abs(spin) < 90 && yOffset > 1 && yOffset < 600 && Math.Abs(xOffset) < 21)
                                            {
                                                this.form.setAligned("down field", System.Drawing.Color.LightYellow);
                                            }
                                            else
                                            {
                                                this.form.setAligned("NOT ALIGNED", System.Drawing.Color.IndianRed);
                                            }
                                        }



                                        // on start roll -> State Takeoff
                                        if (plane.GroundSpeed > config.GroundspeedThreshold && plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Takeoff);
                                            stol.StartTime = DateTime.Now;
                                            this.form.StartStopWatch();

                                            if (config.enableGPXRecodering) 
                                            {
                                                GPXRecorder.Reset();
                                            }

                                            this.stol.ambientTemperature = plane.TemperatureAmbient;
                                            this.stol.ambientPressure = plane.PressureAmbient;
                                        }

                                        // on (vertical) Takeoff -> State Takeoff
                                        if (!plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Climbout);
                                            stol.StartTime = DateTime.Now;
                                            this.form.StartStopWatch();

                                            if (config.enableGPXRecodering)
                                            {
                                                GPXRecorder.Reset();
                                            }
                                        }
                                        break;
                                    }
                                case CycleState.Takeoff:
                                    {
                                        this.form.setAligned("", System.Drawing.SystemColors.Control);
                                        (double andleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);

                                        // on Takeoff -> State Climbout
                                        if (!plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Climbout);
                                            stol.TakeoffPosition = telemetrie.Position;
                                            stol.TakeoffTime = DateTime.Now;
                                            stol.takeoffWindSpeed = plane.AmbientWindSpeed;
                                            stol.takeoffWindDirection = plane.AmbientWindDirection;
                                            // and last 4 chars from UUID
                                            AppendResult($"---- New run {stol.UUID.Substring(stol.UUID.Length - 4)} ---\r\n\r\nTakoff recorded: {(stol.GetTakeoffDistance() * 3.28084):F0} ft");
                                            if (config.debug && config.DebugAutoPause) this.PauseNoPopup(plane, $"Takeoff: {stol.GetTakeoffDistance():F0}ft");

                                            try
                                            {
                                                // send event
                                                if (config.isSendResults) influx.sendEvent(user, stol, plane, "TAKEOFF", (stol.GetTakeoffDistance() * 3.28084).ToString("0"));
                                            }
                                            catch
                                            {
                                                AppendResult("[ERROR]: Unable to send event - check update");
                                            }
                                        }

                                        // stopping -> State Hold
                                        if (plane.IsStopped())
                                        {
                                            // parking or alignment
                                            setState(CycleState.Hold);
                                            this.form.StopStopWatch();
                                        }

                                        break;
                                    }
                                case CycleState.Climbout:
                                    {
                                        var xy = stol.GetDistanceAndOffsetTo(telemetrie.Position);

                                        // lateral field diversion > 90^ or distance > 100ft -> State Fly
                                        // if ((Math.Abs(Math.Round(xy.Item2 * 3.28084)) > 100) ||
                                        if(    (GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading) > 90))
                                        {
                                            setState(CycleState.Fly);
                                            break;
                                        }

                                        // touchdown -> State Takeoff
                                        if (plane.MainGearOnGround())
                                        {
                                            setState(CycleState.Takeoff);
                                            break;
                                        }
                                        break;
                                    }
                                case CycleState.Fly:
                                    {
                                        // Taildragging helper for debugging only
                                        if (plane.TailNoseGearOnGround() && !plane.MainGearOnGround())
                                        {                                         
                                            if (telemetrieLocked == null)
                                            {
                                                if(config.debug && config.DebugAutoPause && config.DebugAutoPauseOnTailTouch) this.PauseNoPopup(plane, "Tail touched");
                                                TimeSpan? time = (DateTime.Now - stol.StartTime);
                                                // time string format mm:ss.sss
                                                string timeStr = time.Value.Minutes.ToString("0") + ":" + time.Value.Seconds.ToString("00") + "." + time.Value.Milliseconds.ToString("000");
                                                AppendResult($"Tail Touchdown recorded {timeStr}: {(this.stol.GetDistanceTo(plane.GetTelemetrie().Position) * 3.28084):F0}ft");
                                                telemetrieLocked = telemetrie;
                                            }
                                        }
                                        else if(telemetrieLocked != null && plane.IsFlapsSet)
                                        {
                                            // continue
                                        }
                                        else
                                        {
                                            if (telemetrieLocked != null) telemetrieLocked = null;
                                        }

                                        // Touchdown -> State Rollout
                                        if (plane.MainGearOnGround())
                                        { 
                                            
                                            // Touchdown!!!
                                            setState(CycleState.Rollout);
                                            stol.planeName = plane.GetDisplayName();
                                            this.stol.planeKey = plane.ConfigKey;
                                            stol.TouchdownPosition = telemetrie.Position;
                                            stol.TouchdownTime = DateTime.Now;
                                            stol.TouchdownPitch = lastTelemetrie.pitch;
                                            stol.TouchdownGs = telemetrie.gForce;
                                            stol.TouchdownGroundSpeed = lastTelemetrie.GroundSpeed;
                                            stol.TouchdownVerticalSpeed = lastTelemetrie.verticalSpeed;
                                            double spin = GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                            stol.maxSpin = Math.Abs(spin);
                                            stol.maxBank = Math.Abs(telemetrie.bank);
                                            double pitch = (double)(stol.InitialPitch - telemetrie.pitch);
                                            stol.minPitch = pitch;
                                            stol.landingWindSpeed = plane.AmbientWindSpeed;
                                            stol.landingWindDirection = plane.AmbientWindDirection;
                                            stol.landingFuelPercent = plane.FuelPercent;


                                            if (config.debug && config.DebugAutoPause) this.PauseAndPopup(plane, $"Touchdown: {(stol.GetTouchdownDistance() * 3.28084):F0}ft");

                                            TimeSpan? time = (DateTime.Now - stol.StartTime);
                                            // time string format mm:ss.sss
                                            string timeStr = time.Value.Minutes.ToString("0") + ":" + time.Value.Seconds.ToString("00") + "." + time.Value.Milliseconds.ToString("000");
                                            AppendResult($"Main Touchdown recorded {timeStr}: {(stol.GetTouchdownDistance() * 3.28084):F0}ft");
                                            AppendResult($"Touchdown registered {timeStr}: {(stol.GetTouchdownDistance() * 3.28084):F0}ft");

                                            (double angleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);
                                            if (spin > angleR + flagsAngleTreshold || spin < angleL - flagsAngleTreshold)
                                            {
                                                // removed ExcessiveTouchdownSpin on request
                                                // stol.violations.Add(new STOLViolation("ExcessiveTouchdownSpin", spin));
                                            }

                                            if (config.isSendResults && stol.hasDeviation("ExcessiveTouchdownSpin"))
                                            {
                                                influx.sendEvent(user, stol, plane, "EXCESSIVE_TOUCHDOWN_SPIN", ((spin).ToString("0.0")));
                                            }

                                            double touchdowndist = stol.GetTouchdownDistance();
                                            if (touchdowndist <= 0)
                                            {
                                                stol.deviations.Add(new STOLDeviation("TouchdownLineViolation", touchdowndist, 3));
                                            }

                                            if (stol.TouchdownGs > plane.GetPlaneConfig().MaxGForce)
                                            {
                                                stol.deviations.Add(new STOLDeviation("ExcessiveGs", (double)stol.TouchdownGs, 2));
                                            }

                                            if (stol.TouchdownVerticalSpeed < plane.GetPlaneConfig().MaxVSpeed)
                                            {
                                                stol.deviations.Add(new STOLDeviation("ExcessiveVerticalSpeed", (double)stol.TouchdownVerticalSpeed, 2));
                                            }


                                            try
                                            {
                                                // send event
                                                if (config.isSendResults)
                                                {
                                                    influx.sendEvent(user, stol, plane, "TOUCHDOWN", (stol.GetTouchdownDistance() * 3.28084).ToString("0"));
                                                }
                                                if (config.isSendResults && stol.hasDeviation("TouchdownLineViolation"))
                                                {
                                                    influx.sendEvent(user, stol, plane, "SCRATCH", (stol.GetTouchdownDistance() * 3.28084).ToString("0"));
                                                }
                                                if (config.isSendResults && stol.hasDeviation("ExcessiveGs"))
                                                {
                                                    influx.sendEvent(user, stol, plane, "EXCESSIVE_G", (((double)stol.TouchdownGs).ToString("0.0")));
                                                }
                                                if (config.isSendResults && stol.hasDeviation("ExcessiveVerticalSpeed"))
                                                {
                                                    influx.sendEvent(user, stol, plane, "EXCESSIVE_VSPEED", (((double)stol.TouchdownVerticalSpeed).ToString("0")));
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                AppendResult("[ERROR]: Unable to send event - check update: " + e.Message);
                                            }

                                            
                                        }
                                        break;
                                    }
                                case CycleState.Rollout:
                                    {
                                        // record attitude on rollout
                                        // double spin = GeoUtils.GetMinDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                        double spin = GeoUtils.GetSignedDeltaAngle((double)stol.InitialHeading, telemetrie.Heading);
                                        stol.maxSpin = Math.Max((double) stol.maxSpin, Math.Abs(spin));
                                        stol.maxBank = Math.Max((double) stol.maxBank, Math.Abs(telemetrie.bank));
                                        double pitch = (double)(stol.InitialPitch - telemetrie.pitch);
                                        stol.minPitch = Math.Min((double)stol.minPitch, pitch);

                                        (double angleL, double angleR) = GetFlagAngles(stol.InitialPosition, (double)stol.InitialHeading, plane);


                                        if (plane.IsParkingBreak)
                                        {
                                            if (!stol.hasDeviation("ParkingBrake"))
                                            {
                                                stol.deviations.Add(new STOLDeviation("ParkingBrake", 1, 1));
                                                if (config.isSendResults)
                                                {
                                                    influx.sendEvent(user, stol, plane, "PARKING_BRAKE", "true");
                                                }
                                            }
                                        }

                                        // stopping -> Hold
                                        // checking for save nose up attitude
                                        if (plane.IsStopped() && plane.pitch < 10) // && plane.TailNoseGearOnGround())
                                        {
                                            setState(CycleState.Hold);
                                            stol.StopPosition = telemetrie.Position;
                                            stol.StopTime = DateTime.Now;
                                            this.form.StopStopWatch();

                                            if (spin > angleR + flagsAngleTreshold || spin < angleL - flagsAngleTreshold)
                                            {
                                                stol.deviations.Add(new STOLDeviation("ExcessiveStopSpin", (double)spin, 2));
                                            }

                                            if (Math.Abs((double)stol.maxSpin) > 45.0)
                                            {
                                                stol.deviations.Add(new STOLDeviation("ExcessiveMaxSpin", (double)stol.maxSpin, 1));
                                            }

                                            // End Cycle
                                            STOLResult result = stol.GetResult(unit);
                                            AppendResult(result.getConsoleString());
                                            form.DrawResult(result);

                                            try
                                            {
                                                // send event
                                                if (config.isSendResults)
                                                {
                                                    influx.sendEvent(user, stol, plane, "STOP", (stol.GetLandingDistance() * 3.28084).ToString("0"));
                                                }

                                                if (config.isSendResults && stol.hasDeviation("ExcessiveStopSpin"))
                                                {
                                                    influx.sendEvent(user, stol, plane, "EXCESSIVE_STOP_SPIN", spin.ToString("0.0"));
                                                }

                                                if (config.isSendResults && stol.hasDeviation("ExcessiveMaxSpin"))
                                                {
                                                    influx.sendEvent(user, stol, plane, "EXCESSIVE_MAX_SPIN", ((double)stol.maxSpin).ToString("0.0"));
                                                }
                                            }
                                            catch
                                            {
                                                AppendResult("[ERROR]: Unable to send event - check update");
                                            }


                                            lastStol = stol;
                                            stol.Reset();
                                            try
                                            {
                                                var dir = Path.GetDirectoryName(config.ResultsExportPath);
                                                Directory.CreateDirectory(dir);
                                                using (StreamWriter writer = new StreamWriter(config.ResultsExportPath, append: true))
                                                {
                                                    writer.WriteLine(result.getCsvString());
                                                }
                                            }
                                            catch
                                            {
                                                AppendResult("[ERROR]: Unable to export to " + config.ResultsExportPath);
                                            }
                                            try
                                            {
                                                // send influx
                                                if (config.isSendResults && user != null && user != "") influx.sendData(result);
                                            }
                                            catch
                                            {
                                                AppendResult("[ERROR]: Unable to send result - check update");
                                            }

                                            // save gpx
                                            if (config.enableGPXRecodering)
                                            {
                                                GPXRecorder.Append(telemetrie);
                                                GPXRecorder.Save(stol.user.Trim(), stol.planeName);
                                            }
                                        }
                                        // Alt AGL > 5 ft to avoid bounce detection
                                        if (!plane.MainGearOnGround() && plane.AltitudeAGL > 10)
                                        {
                                            // touch n go
                                            setState(CycleState.Fly);
                                            AppendResult("Touch 'n' go recorded, try Again");
                                            stol.deviations.Add(new STOLDeviation("TouchNGo", 1, 0));
                                            if (config.isSendResults) influx.sendEvent(user, stol, plane, "TOUCH_N_GO", "true");
                                        }
                                        break;
                                    }
                            }

                            if (config.enableGPXRecodering && cycleState != CycleState.Hold && cycleState != CycleState.Unknown)
                            {
                                GPXRecorder.Append(telemetrie);
                            }
                        }
                        else
                        {
                            // wait until stol is init
                            if(this.form != null) this.form.setAligned("No Reference: Apply Preset", System.Drawing.Color.Red);
                        }
                        lastTelemetrie = telemetrie;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                // sllep until next interval
                int intervall = plane.IsSimConnected ? config.RefreshInterval : config.IdleRefreshInterval;
                System.Threading.Thread.Sleep(intervall);
            };
        }

        void WriteResult(string text) 
        {
            Console.WriteLine(text);
            if(form != null) form.setResult(text);
        }

        void AppendResult(string text)
        {
            Console.WriteLine(text);
            if (form != null) form.appendResult(text);
        }

        private (double, double) GetFlagAngles(GeoCoordinate origin, double initHeading, Plane plane)
        {
            (double yOffset, double xOffset) = GeoUtils.GetDistanceAlongAxis(origin, plane.getPositionWithGearOffset(), initHeading);

            //if (config.debug) Console.WriteLine($"x: {xOffset:F1} m  y: {yOffset:F1} m");

            double adjacent = 182.88 - yOffset;
            double halfSpan = 42.672 / 2;

            double oppositeLeft = halfSpan + xOffset;
            double oppositeRight = halfSpan - xOffset;

            //if (config.debug) Console.WriteLine($"adjacent: {adjacent:F1} m  opposites: L={oppositeLeft:F1} m  R={oppositeRight:F1} m");

            double angleLeft = Math.Atan(oppositeLeft / adjacent) * (180.0 / Math.PI);
            double angleRight = Math.Atan(oppositeRight / adjacent) * (180.0 / Math.PI);

            //if (config.debug) Console.WriteLine($"angleL: {angleLeft:F1}°  angleR: {angleRight:F1}°");

            return (-angleLeft, angleRight);
        }



        private void setState(CycleState state)
        {
            var old = cycleState;
            cycleState = state;
            if (form != null) 
            {
                form.setState(state.ToString());
            } 
        }

        public void OnPlaneEventCallback(PlaneEvent evt)
        {
            // Console.WriteLine(evt.Event);
            switch (evt.Event)
            {
                case "PARKING_BRAKES":
                    {
                        if (openWorldMode && plane.IsStopped())
                        {
                            // initSTOL();
                        }
                        break;
                    }
                case "SMOKE_TOGGLE":
                    {
                        if (openWorldMode && plane.IsStopped())
                        {
                            // initSTOL();
                        }
                        break;
                    }
            }
        }

        public async void PauseAndPopup(Plane plane, string message = "Sim paused") 
        {
            // Pause immediately and mark paused state
            plane.Pause();
            this.isPaused = true;

            // If we have the UI form, schedule the MessageBox on the UI thread without blocking the caller.
            if (form != null && form.IsHandleCreated)
            {
                // BeginInvoke returns immediately; the lambda runs on the UI thread and shows the modal MessageBox there.
                form.BeginInvoke(new Action(() =>
                {
                    var result = MessageBox.Show(form, message, "Sim paused", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (result == DialogResult.OK)
                    {
                        plane.Unpause();
                        this.isPaused = false;
                    }
                }));
                return;
            }
        }

        public async void PauseNoPopup(Plane plane, string message = "Sim paused")
        {
            plane.Pause();
            this.isPaused = true;
            if (form != null)
            {
                form.appendResult(message);
            }
        }

        public void Unpause()
        {
            plane.Unpause();
            this.isPaused = false;
        }

        public void SetPreset(string presetTitle) 
        {
            if (presetTitle == "Open World") 
            {
                preset = null;
                stol.Reset();
                stol.ApplyOpenWorld(plane);
                openWorldMode = true;
                return;
            }

            var selectedPreset = presets.Where(p => p.title == presetTitle).FirstOrDefault();

            preset = selectedPreset;
            openWorldMode = false;
            stol.Reset();
            stol.ApplyPreset(preset);
            UILogger.LogInfo($"STOL cycle initiated: {stol.GetConfigHash().Substring(0, 4)}... ");
            UILogger.LogInfo($"START: { GeoUtils.ConvertToDMS(stol.InitialPosition)} HDG: { Math.Round(stol.InitialHeading.Value)}°");
            string hasConfigText = plane.HasPlaneConfig() ? "config found" : "no config";
            UILogger.LogInfo($"Plane set: {plane.GetDisplayName()} {hasConfigText}");

            UILogger.LogDebug($"[DEBUG]: Using:\r\nType: \"{plane.Type}\"\r\nModel: \"{plane.Model}\"\r\nTitle: \"{plane.Title}\"\r\nConfig: {plane.HasPlaneConfig()} (\"{plane.GetDisplayName()}\") ");
        }

        public void AutoSetPreset()
        {
            GeoCoordinate planePosition = plane.GetTelemetrie().Position;

            Preset selectedPreset = presets[0];
            double minDistance = double.MaxValue;
            foreach (Preset p in presets) 
            {
                GeoCoordinate Line = new GeoCoordinate(p.startLatitude, p.startLongitude);
                double distance = planePosition.GetDistanceTo(Line);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedPreset = p;
                }
            }

            AppendResult($"Nearest Reference Line selected: {selectedPreset.title}");
            form.setSelctedPreset(selectedPreset.title);
        }

        public void SetStartPos()
        {
            initSTOL();
        }

        public bool IsSimConnected()
        {
            return this.plane.isInit;
        }

        public bool IsStolInit() 
        {
            return stol.IsInit();
        }

        public bool setFuelStandard()
        {
            double fuel = config.FuelLevelStandard;
            plane.SetFuelPercent(fuel);
            AppendResult($"fuel set {fuel*100}%");
            influx.sendEvent(user, stol, plane, "FUEL SET", (fuel*100).ToString());
            return true;
        }

        private void initSTOL()
        {
            // set STOL initial Values
            stol.planeName = plane.GetDisplayName();
            this.stol.planeKey = plane.ConfigKey;

            string hasConfigText = plane.HasPlaneConfig() ? "config found" : "no config";
            this.AppendResult($"Plane set: {plane.GetDisplayName()} {hasConfigText}");

            stol.InitialHeading = plane.Heading;
            stol.InitialPitch = plane.pitch;
            stol.InitialPosition = plane.GetTelemetrie().Position;
            setState(CycleState.Hold);
            UILogger.LogDebug($"[DEBUG]: STOL cycle initiated\nSTART: {GeoUtils.ConvertToDMS(stol.InitialPosition)} HDG: {Math.Round(stol.InitialHeading.Value)}°");
            UILogger.LogDebug($"[DEBUG]: Using:\nType: \"{plane.Type}\"\nModel: \"{plane.Model}\"\nTitle: \"{plane.Title}\"\n");
        }

        public void TeleportToReferenceLine() 
        {
            bool teleportWhileFlying = !plane.MainGearOnGround() || plane.GroundSpeed > config.GroundspeedThreshold;

            if (teleportWhileFlying && config.ForcePauseOnTeleportFromMoving)
            {
                plane.setPosition(stol.InitialPosition, stol.InitialHeading ?? 0, true, 2);
                this.PauseNoPopup(plane, "Teleported to Reference Line while flying.\r\nThrottle down, Brakes, get ready! Unpause [►]");
                plane.resetSpeed();
            } else 
            {
                plane.setPosition(stol.InitialPosition, stol.InitialHeading ?? 0, false, 2);
                if (config.PauseOnTeleport)
                {
                    this.PauseNoPopup(plane, "Teleported to Reference Line.\r\nThrottle down, Brakes, get ready! Unpause [►]");
                }
                plane.resetSpeed();
            }
        }

        public void reloadPresets()
        {
            presets = Preset.ReadPresets(config.PresetsPath, config.CustomPresetsPath);
            if (form != null)
            {
                form.setPresets(presets.Select(p => p.title).ToArray());
            }
            PlaneConfigsService.LoadPlaneConfigs();
            stol.Reset();
        }

        internal void unflip()
        {
            if (plane.IsOnGround && plane.GroundSpeed < config.GroundspeedThreshold) 
            {
                influx.sendEvent(user, stol, plane, "UNFLIP");
                plane.setDoubleValue("ROTATION VELOCITY BODY X", -3);
            }
        }
    }
}
