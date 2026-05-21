using Newtonsoft.Json;
using System;
using System.Device.Location;

namespace STOL_Training_Tool
{

    public class Telemetrie
    {
        /// <summary>
        /// Aircraft reference position (lat, lon, alt)
        /// </summary>
        [JsonProperty("position")]
        public GeoCoordinate Position { get; set; }

        /// <summary>
        /// Center of gravity position (lat, lon, alt)
        /// </summary>
        [JsonProperty("position_cg")]
        public GeoCoordinate PositionCG { get; set; }

        /// <summary>
        /// Height above ground level (AGL)
        /// </summary>
        [JsonProperty("height")]
        public double Height { get; set; } = 0;

        /// <summary>
        /// Physical altitude above sea level (ASL, ft)
        /// </summary>
        [JsonProperty("alt")]
        public double Altitude { get; set; }

        /// <summary>
        /// Physical altitude above ground level (AGL, ft)
        /// </summary>
        [JsonProperty("alt_agl")]
        public double AltitudeAGL { get; set; }

        /// <summary>
        /// Ground speed in knots
        /// </summary>
        [JsonProperty("ground_speed")]
        public double GroundSpeed { get; set; }

        /// <summary>
        /// Airspeed in knots
        /// </summary>
        [JsonProperty("air_speed")]
        public double AirSpeed { get; set; }

        /// <summary>
        /// True heading in degrees
        /// </summary>
        [JsonProperty("heading")]
        public double Heading { get; set; }

        /// <summary>
        /// Velocity along aircraft X-axis (m/s)
        /// </summary>
        [JsonProperty("vx")]
        public double vX { get; set; }

        /// <summary>
        /// Velocity along aircraft Y-axis (m/s)
        /// </summary>
        [JsonProperty("vy")]
        public double vY { get; set; }

        /// <summary>
        /// Velocity along aircraft Z-axis (m/s)
        /// </summary>
        [JsonProperty("vz")]
        public double vZ { get; set; }

        /// <summary>
        /// Pitch angle (degrees)
        /// </summary>
        [JsonProperty("pitch")]
        public double pitch { get; set; } = 0;

        /// <summary>
        /// Bank (roll) angle (degrees)
        /// </summary>
        [JsonProperty("bank")]
        public double bank { get; set; } = 0;

        /// <summary>
        /// Vertical speed in feet per minute
        /// </summary>
        [JsonProperty("vertical_speed")]
        public double verticalSpeed { get; set; } = 0;

        /// <summary>
        /// G-force (relative to 1g)
        /// </summary>
        [JsonProperty("g_force")]
        public double gForce { get; set; } = 0;

        /// <summary>
        /// Main wheel RPM
        /// </summary>
        [JsonProperty("main_wheel_rpm")]
        public double mainWheelRPM { get; set; } = 0;

        /// <summary>
        /// Center wheel RPM
        /// </summary>
        [JsonProperty("center_wheel_rpm")]
        public double centerWheelRPM { get; set; } = 0;

        /// <summary>
        /// Flap deflection percentage (0–100)
        /// </summary>
        [JsonProperty("flaps_percent")]
        public double FlapsPercent { get; set; } = 0;

        /// <summary>
        /// Flaps handle position index
        /// </summary>
        [JsonProperty("flaps_handle_position")]
        public uint FlapsHandlePosition { get; set; } = 0;

        /// <summary>
        /// Aileron deflection percentage
        /// </summary>
        [JsonProperty("ailerons_percent")]
        public double AileronsPercent { get; set; } = 0;

        /// <summary>
        /// Elevator deflection percentage
        /// </summary>
        [JsonProperty("elevators_percent")]
        public double ElevatorsPercent { get; set; } = 0;

        /// <summary>
        /// Rudder deflection percentage
        /// </summary>
        [JsonProperty("rudder_percent")]
        public double RudderPercent { get; set; } = 0;

        /// <summary>
        /// Throttle lever position (0–100%)
        /// </summary>
        [JsonProperty("throttle_position")]
        public double ThrottlePosition { get; set; } = 0;

        /// <summary>
        /// True if aircraft is on ground
        /// </summary>
        [JsonProperty("on_ground")]
        public bool OnGround { get; set; } = false;

        /// <summary>
        /// Boolean array of contact point states
        /// </summary>
        [JsonProperty("contact_points")]
        public bool[] ContactPoints { get; set; } = new bool[21];

        /// <summary>
        /// wind vector X component (knots)
        /// </summary>
        [JsonProperty("wind_x")]
        public double WindX { get; set; } = 0.0;

        /// <summary>
        /// wind vector Y component (knots)
        /// </summary>
        [JsonProperty("wind_y")]
        public double WindY { get; set; } = 0.0;

        public override string ToString()
        {
            return $"[{GeoUtils.ConvertToDMS(Position)}], {Math.Round(Altitude)} ft, {Math.Round(Heading)}°, {Math.Round(GroundSpeed)} knts";
        }
    }
}
