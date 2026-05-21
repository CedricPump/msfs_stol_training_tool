using System;
using System.Device.Location;
using System.Reflection;
using System.Threading.Tasks;
using STOL_Training_Tool.Model;
using STOL_Training_Tool_Core.Core;
using STOL_Training_Tool_Core.Model;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace STOL_Training_Tool
{
    public abstract class Aircraft
    {
        public bool isInit = false;

        // Ident
        public string Model { get; protected set; }
        public string Type { get; protected set; }
        public string Title { get; protected set; }
        public string ConfigKey { get; protected set; } = null;
        // Position
        public double Altitude { get; protected set; }
        public double Height_AGL { get; protected set; }
        public double Latitude { get; protected set; }
        public double Longitude { get; protected set; }
        public double Heading { get; protected set; }
        // Movement
        public double GroundSpeed { get; protected set; }
        public double VerticalSpeed { get; protected set; }
        public double vX { get; protected set; }
        public double vY { get; protected set; }
        public double vZ { get; protected set; }
        public double gforce { get; protected set; }
        public double Airspeed { get; protected set; }

        public double RWheelRPM { get; protected set; }
        public double LWheelRPM { get; protected set; }
        public double CWheelRPM { get; protected set; }
        public double AuxWheelRPM { get; protected set; }

        // Orientation
        public double pitch { get; protected set; }
        public double bank { get; protected set; }
        // State
        public bool IsOnRundway { get; protected set; }
        public bool IsOnGround { get; protected set; }
        public bool IsEngineOn { get; protected set; }
        public bool IsParkingBreak { get; protected set; }
        public double Fuel { get; protected set; }
        public double FuelPercent { get; protected set; }
        public bool FuelUnlimited { get; protected set; }
        public string Airport { get; protected set; }
        public bool[] ContactPoints { get; protected set; } = new bool[21];

        public double PropRPM { get; protected set; } = 0.0;

        // Env
        public double AltitudeAGL { get; protected set; }

        // Sim
        public bool IsSimConnected { get; protected set; } = false;
        public bool SimDisabled { get; protected set; } = false;
        // AntiCheat
        public bool CrashEnabled { get; protected set; } = false;
        public bool IsSlew { get; protected set; } = false;
        public int TimeAcceleration { get; private set; } = 1;
        // Weight
        public double PilotWeight { get; protected set; }
        public double TotalWeight { get; protected set; }
        public double MaxTotalWeight { get; protected set; }

        // Ambient
        public double WindX { get; protected set; } = 0.0;
        public double WindY { get; protected set; } = 0.0;
        public double AmbientWindX { get; protected set; } = 0.0;
        public double AmbientWindY { get; protected set; } = 0.0;
        public double AmbientWindDirection { get; protected set; } = 0.0;
        public double AmbientWindSpeed { get; protected set; } = 0.0;
        public double Antistall { get; protected set; } = 0;
        public bool Autotrim { get; protected set; } = false;
        public bool AICtrl { get; protected set; } = false;

        public double TemperatureAmbient { get; protected set; } = 0.0;
        public double PressureAmbient { get; protected set; } = 0.0;


        // flight controls
        public double AileronPosition { get; protected set; } = 0.0;
        public double ElevatorPosition { get; protected set; } = 0.0;
        public double RudderPosition { get; protected set; } = 0.0;
        public double FlapsPercent { get; protected set; } = 0.0;
        public uint FlapsIndex { get; protected set; } = 0;
        public double ThrottlePosition { get; protected set; } = 0.0;

        public bool IsFlapsSet { get { return FlapsPercent > 0; } }

        public bool IsVNEOverspeed { get; protected set; } = false;
        public bool IsFlapsOverspeed { get; protected set; } = false;

        protected Config conf;

        public bool IsTaildragger { get
            {
                return GetPlaneConfig().IsTaildragger;
            }
        }

        public PlaneConfig GetPlaneConfig() 
        {
            if(!HasPlaneConfigKey())
            {
                this.ConfigKey = PlaneConfigsService.GetPlaneConfigKey(this.GetIdent());
            }
            return PlaneConfigsService.GetPlaneConfig(this.ConfigKey);
        }

        public bool HasPlaneConfigKey()
        {
            return this.ConfigKey != null || this.ConfigKey == "";
        }

        public bool HasPlaneConfig()
        {
            return HasPlaneConfigKey() && this.ConfigKey != "DEFAULT";
        }

        public string GetDisplayName() 
        {
            var dispalyName = GetPlaneConfig().DisplayName;
            if (GetPlaneConfig().Key == "DEFAULT")
            {
                dispalyName = $"{GetPlaneConfig().DisplayName} no config ({this.Title})";
            }
            return string.IsNullOrEmpty(dispalyName) ? this.Title : dispalyName;
        }

        public string toString()
        {
            return Model + " [" + Latitude + "," + Longitude + "," + Altitude + "] " + GroundSpeed + "knts " + Heading + "° ";
        }

        public enum EVENTS
        {
            SimStart,
            SimStop,
            Crashed,
            AircraftLoaded,
            FlightLoaded,
            LANDING_LIGHTS_TOGGLE,
            PARKING_BRAKES,
            PARKING_BRAKE_SET,
            SMOKE_OFF,
            SMOKE_ON,
            SMOKE_SET,
            SMOKE_TOGGLE,
            TOW_PLANE_RELEASE,
            HORN_TRIGGER,
            PAUSE_TOGGLE,
            PAUSE_ON,
            PAUSE_OFF,
            PAUSE_SET,
            SIM_RATE,
            SIM_RATE_DECR,
            SIM_RATE_INCR,
            SIM_RATE_SET
        };

        public Telemetrie GetTelemetrie()
        {
            return new Telemetrie
            {
                Position = getPositionWithGearOffset(),
                PositionCG = getPosition(),
                Altitude = this.Altitude,
                AltitudeAGL = this.AltitudeAGL,
                Height = 0.0,
                GroundSpeed = this.GroundSpeed,
                Heading = this.Heading,
                vX = this.vX,
                vY = this.vY,
                vZ = this.vZ,
                pitch = this.pitch,
                bank = this.bank,
                verticalSpeed = this.VerticalSpeed,
                gForce = this.gforce,
                mainWheelRPM = Math.Max(this.RWheelRPM, this.LWheelRPM),
                centerWheelRPM = this.CWheelRPM,
                FlapsPercent = this.FlapsPercent,
                FlapsHandlePosition = this.FlapsIndex,
                AileronsPercent = this.AileronPosition,
                ElevatorsPercent = this.ElevatorPosition,
                RudderPercent   = this.RudderPosition,
                OnGround = this.IsOnGround,
                AirSpeed = this.Airspeed,
                ThrottlePosition = this.ThrottlePosition,
                WindX = this.WindX,
                WindY = this.WindY,
            };
        }

        private GeoCoordinate getPosition()
        {
            return new GeoCoordinate(this.Latitude, this.Longitude, this.Altitude * 0.3048);
        }

        public GeoCoordinate getPositionWithGearOffset() 
        {
            double offset = PlaneConfigsService.GetGearOffset(this.ConfigKey);
            GeoCoordinate simPos = new GeoCoordinate(this.Latitude, this.Longitude, this.Altitude * 0.3048);
            return GeoUtils.GetOffsetPosition(simPos, this.Heading, -offset);
        }

        public Ident GetIdent()
        {
            return new Ident
            {
                Model = this.Model,
                Title = this.Title,
                Type = this.Type,
            };
        }

        public bool IsFlipped()
        {
            return (this.IsStopped() || this.IsOnGround) && this.pitch >= this.GetPlaneConfig().PropStrikeThreshold;
        }

        public bool IsStopped()
        {
            return this.GroundSpeed < Config.GetInstance().GroundspeedThreshold && this.MainGearOnGround();
        }

        public bool MainGearOnGround()
        {
            return LeftGearOnGround() || RightGearOnGround();
        }

        public bool LeftGearOnGround()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelLeftIndex];
        }

        public bool RightGearOnGround()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelRightIndex];
        }

        public bool TailNoseGearOnGround()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelNoseTailIndex];
        }

        public bool WingtipOnGround()
        {
            return WingtipOnGroundR() || WingtipOnGroundL();
        }

        public bool WingtipOnGroundR()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelWingtipRIndex];
        }

        public bool WingtipOnGroundL()
        {
            return ContactPoints[this.GetPlaneConfig().CollisionWheelWingtipLIndex];
        }

        public bool IsPropstrike()
        {
            int index = this.GetPlaneConfig().CollisionPropIndex;
            bool collisionProp = false;
            if (index >= 0)
            {
                collisionProp = ContactPoints[index];
            }
            bool propStrikeAngleReached = this.IsOnGround && this.pitch > this.GetPlaneConfig().PropStrikeThreshold;
            return collisionProp || propStrikeAngleReached;
        }

        public double getWindTotal()
        {
            double windTotal = Math.Sqrt(WindX * WindX + WindY * WindY);
            return windTotal;
        }

        public double getRelWindDir()
        {
            double angleRad = Math.Atan2(WindX, -WindY); // flip windX to get tailwind at 0°
            double angleDeg = angleRad * (180.0 / Math.PI);

            // Normalize to [0, 360)
            if (angleDeg < 0)
                angleDeg += 360;

            return angleDeg;
        }

        public double GetWindDirectionRelativeRL()
        {
            // relative wind direction to aircraft heading
            double relDir = getRelWindDir();
            // return relative wind left or right 
            // wind from right 0° to  +180° wind from left 0° to -180°
            // crosswind is +-90°
            return relDir;
        }

        public AircraftState GetState()
        {
            return new AircraftState
            {
                EngineOn = this.IsEngineOn,
                Fuel = this.Fuel,
                FuelPercent = this.FuelPercent,
                ParkingBrake = this.IsParkingBreak,
                Weight = this.TotalWeight,
                MaxWeightPercent = this.TotalWeight / this.MaxTotalWeight * 100,
                PilotWeight = this.PilotWeight,
                FuelUnlimited = this.FuelUnlimited,
            };
        }



        public abstract bool Update();

        public abstract void OnEvent(EVENTS recEvent);

        public abstract void OnQuit();

        public abstract bool setDoubleValue(string name, double value);
        public abstract bool setBoolValue(string name, bool value);
        public abstract bool setIntValue(string name, int value);

        public abstract void Pause();

        public abstract void Unpause();

        public abstract void sendEvent(EVENTS myEvent, uint dwData = 1);

        public void setPosition(GeoCoordinate position, double heading, bool setAttitude = false, double altitudeOffset = 0.0)
        {
            double offset = PlaneConfigsService.GetGearOffset(this.ConfigKey);

            GeoCoordinate offsetPos = GeoUtils.GetOffsetPosition(position, heading, offset);

            this.setBoolValue("SIM DISABLED", true);
            this.setDoubleValue("PLANE LATITUDE", offsetPos.Latitude);
            this.setDoubleValue("PLANE LONGITUDE", offsetPos.Longitude);
            this.setDoubleValue("PLANE ALTITUDE", offsetPos.Altitude + altitudeOffset);
            this.setDoubleValue("PLANE HEADING DEGREES TRUE", heading);
            //this.resetSpeed();
            if (setAttitude)
            {
                this.setDoubleValue("PLANE PITCH DEGREES", 0.0);
                this.setDoubleValue("PLANE BANK DEGREES",  0.0);
            }
            System.Threading.Thread.Sleep(333);
            this.setBoolValue("SIM DISABLED", false);
        }

        public bool SetFuelPercent(double percent) 
        {
            bool res = true;
            // res &= this.setDoubleValue("FUELSYSTEM TANK LEVEL: 0", percent);
            res &= this.setDoubleValue("FUELSYSTEM TANK LEVEL: 1", percent);
            res &= this.setDoubleValue("FUELSYSTEM TANK LEVEL: 2", percent);
            res &= this.setDoubleValue("FUELSYSTEM TANK LEVEL: 3", percent);
            res &= this.setDoubleValue("FUELSYSTEM TANK LEVEL: 4", percent);
            return res;
        }


        public void resetSpeed()
        {
            this.setDoubleValue("VELOCITY WORLD X", 0.0);
            this.setDoubleValue("VELOCITY WORLD Y", 0.0);
            this.setDoubleValue("VELOCITY WORLD Z", 0.0);
            this.setDoubleValue("GROUND VELOCITY",  0.0);
            //this.setValue("AIRSPEED INDICATED", 0);
        }

        public abstract void SpawnObject(string objectName, double latitude, double longitude, double altitude);
    }
}
