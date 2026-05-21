namespace STOL_Training_Tool.Model
{
    public class AircraftState
    {
        public bool EngineOn { get; set; }
        public bool ParkingBrake { get; set; }
        public string Airport { get; set; }
        public bool OnGround { get; set; }
        public double Fuel { get; set; }
        public double FuelPercent { get; set; }
        public bool FuelUnlimited { get; set; }
        public double PilotWeight { get; set; }
        public double Weight { get; set; }
        public double MaxWeightPercent { get; set; }
    }
}
