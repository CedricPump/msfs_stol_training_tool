using System;
using System.Collections.Generic;
using STOL_Training_Tool_Core.Core;
using Microsoft.FlightSimulator.SimConnect;

namespace STOL_Training_Tool
{
    public class AircraftSimConnect : Plane
    {
        private SimConnect simconnect;
        private IntPtr m_hWnd = new IntPtr(0);
        private const int WM_USER_SIMCONNECT = 0x0402;
        private Dictionary<DATA_DEFINE_ID, DataDefinition> definitions = new Dictionary<DATA_DEFINE_ID, DataDefinition>();
        private Dictionary<string, DataDefinition> definitions_by_string = new Dictionary<string, DataDefinition>();

        const uint SIMCONNECT_OBJECT_ID_USER = 0;

        enum GROUP_ID
        {
            GROUP_A
        };

        public static EVENTS[] SystemEvents = new EVENTS[] {
            EVENTS.SimStart,
            EVENTS.SimStop,
            EVENTS.Crashed,
            EVENTS.AircraftLoaded,
            EVENTS.FlightLoaded
        };


        public AircraftSimConnect(PlaneEventCallBack callback) : base(callback)
        {
            this.IsSimConnected = false;
            this.conf = Config.GetInstance();
            ConnectSimConnect();
            // simconnect.Text(SIMCONNECT_TEXT_TYPE.SCROLL_GREEN, 5.0f, null, "STOL_Training_Tool connected");
        }

        private void InitSimConnect(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            definitions = new Dictionary<DATA_DEFINE_ID, DataDefinition>();
            definitions_by_string = new Dictionary<string, DataDefinition>();

            Console.Write("simconnect init ");
            // Identity
            CreateDataDefinition("ATC MODEL", "", true);
            CreateDataDefinition("ATC TYPE", "", true);
            CreateDataDefinition("TITLE", "", true);
            // Position
            CreateDataDefinition("PLANE ALTITUDE", "feet");
            CreateDataDefinition("PLANE LATITUDE", "degrees");
            CreateDataDefinition("PLANE LONGITUDE", "degrees");
            // Orientation
            CreateDataDefinition("PLANE BANK DEGREES", "degrees");
            CreateDataDefinition("PLANE PITCH DEGREES", "degrees");
            CreateDataDefinition("PLANE HEADING DEGREES TRUE", "degrees");
            // CreateDataDefinition("PLANE HEADING DEGREES MAGNETIC", "degrees");
            // Speed
            CreateDataDefinition("GROUND VELOCITY", "knots");
            CreateDataDefinition("AIRSPEED INDICATED", "knots");
            // CreateDataDefinition("AIRSPEED TRUE", "knots");

            //FlightControls
            CreateDataDefinition("AILERON POSITION", "position");
            CreateDataDefinition("ELEVATOR POSITION", "position");
            CreateDataDefinition("RUDDER POSITION", "position");
            CreateDataDefinition("FLAPS HANDLE PERCENT", "percent");
            CreateDataDefinition("FLAPS HANDLE INDEX", "number");

            //CreateDataDefinition("RIGHT WHEEL RPM", "RPM");
            //CreateDataDefinition("LEFT WHEEL RPM", "RPM");
            //CreateDataDefinition("CENTER WHEEL RPM", "RPM");
            //CreateDataDefinition("AUX WHEEL RPM", "RPM");

            CreateDataDefinition("VERTICAL SPEED", "feet per minute");
            CreateDataDefinition("G FORCE", "Gforce");
            CreateDataDefinition("VELOCITY WORLD X", "meter per second");
            CreateDataDefinition("VELOCITY WORLD Y", "meter per second");
            CreateDataDefinition("VELOCITY WORLD Z", "meter per second");
            // Anti-Cheat
            CreateDataDefinition("REALISM CRASH DETECTION", "");
            CreateDataDefinition("IS SLEW ACTIVE", "");
            // State
            CreateDataDefinition("ENG COMBUSTION:1", "Bool");
            CreateDataDefinition("GENERAL ENG COMBUSTION:0", "Bool");
            CreateDataDefinition("GENERAL ENG COMBUSTION:1", "Bool");
            CreateDataDefinition("GENERAL ENG COMBUSTION:2", "Bool");
            CreateDataDefinition("BRAKE PARKING POSITION", "Bool");
            CreateDataDefinition("GEAR IS ON GROUND", "Bool");
            CreateDataDefinition("SIM ON GROUND", "Bool");
            CreateDataDefinition("ON ANY RUNWAY", "Bool");
            CreateDataDefinition("PROP RPM:1", "RPM");

            //CreateDataDefinition("NAV LOC AIRPORT IDENT", "", true);
            // Environment
            CreateDataDefinition("PLANE ALT ABOVE GROUND MINUS CG", "feet");
            // Fuel
            CreateDataDefinition("FUEL TOTAL QUANTITY WEIGHT", "pounds");
            CreateDataDefinition("FUEL SELECTED QUANTITY PERCENT", "percent over 100");
            CreateDataDefinition("FUELSYSTEM TANK LEVEL: 0", "percent over 100");
            CreateDataDefinition("FUELSYSTEM TANK LEVEL: 1", "percent over 100");
            CreateDataDefinition("FUELSYSTEM TANK LEVEL: 2", "percent over 100");
            CreateDataDefinition("FUELSYSTEM TANK LEVEL: 3", "percent over 100");
            CreateDataDefinition("FUELSYSTEM TANK LEVEL: 4", "percent over 100");


            CreateDataDefinition("UNLIMITED FUEL", "Bool");

            // Action
            CreateDataDefinition("SMOKE ENABLE", "Bool");
            CreateDataDefinition("SIM DISABLED", "Bool");
            // Payload
            //CreateDataDefinition("PAYLOAD STATION COUNT", "number");

            // Pilot Weight
            CreateDataDefinition("PAYLOAD STATION WEIGHT:1", "lbs");


            //CreateDataDefinition("PAYLOAD STATION WEIGHT:1", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:2", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:3", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:4", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:5", "lbs");
            //
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:6", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:7", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:8", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:9", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:10", "lbs");
            //
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:11", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:12", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:13", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:14", "lbs");
            //CreateDataDefinition("PAYLOAD STATION WEIGHT:15", "lbs");

            CreateDataDefinition("CONTACT POINT IS ON GROUND:0", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:1", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:2", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:3", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:4", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:5", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:6", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:7", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:8", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:9", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:10", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:11", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:12", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:13", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:14", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:15", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:16", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:17", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:18", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:19", "Bool");
            CreateDataDefinition("CONTACT POINT IS ON GROUND:20", "Bool");


            CreateDataDefinition("TOTAL WEIGHT", "lbs");
            CreateDataDefinition("MAX GROSS WEIGHT", "lbs");

            // Ambient
            CreateDataDefinition("AIRCRAFT WIND X", "knots");
            CreateDataDefinition("AIRCRAFT WIND Z", "knots");

            CreateDataDefinition("AMBIENT WIND DIRECTION", "degrees");
            CreateDataDefinition("AMBIENT WIND VELOCITY", "knots");

            CreateDataDefinition("AMBIENT PRESSURE", "mbar");
            CreateDataDefinition("AMBIENT TEMPERATURE", "Celsius");

            CreateDataDefinition("ASSISTANCE LANDING ENABLED", "Bool");
            CreateDataDefinition("ASSISTANCE TAKEOFF ENABLED", "Bool");
            CreateDataDefinition("AI ANTISTALL STATE", "number");
            CreateDataDefinition("AI AUTOTRIM ACTIVE", "number");
            CreateDataDefinition("AI CONTROLS", "number");

            CreateDataDefinition("ROTATION VELOCITY BODY X", "Feet per second");
            CreateDataDefinition("FLAP SPEED EXCEEDED", "Bool");
            CreateDataDefinition("OVERSPEED WARNING", "Bool");

            RegiserDefinitions();

            this.isInit = true;
        }

        private void RegiserDefinitions()
        {
            foreach (DataDefinition def in definitions.Values)
            {
                RegisterDataDefinition(def);
            }
        }

        private DataDefinition CreateDataDefinition(string name, string unit = "", bool isString = false)
        {
            DataDefinition def = new DataDefinition(name, unit, isString);
            this.definitions.Add(def.defId, def);
            this.definitions_by_string.Add(name, def);
            return def;
        }

        private void RegisterDataDefinition(DataDefinition def)
        {
            if (def.isString)
            {
                simconnect.AddToDataDefinition(def.defId, def.dname, "", SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.RegisterDataDefineStruct<DataStruct>(def.defId);
                simconnect.RequestDataOnSimObjectType(def.reqId, def.defId, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
            else
            {
                simconnect.AddToDataDefinition(def.defId, def.dname, def.dunit, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                simconnect.RegisterDataDefineStruct<double>(def.defId);
                simconnect.RequestDataOnSimObjectType(def.reqId, def.defId, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            }
        }

        private DataDefinition getDefinitionByName(string name)
        {
            DataDefinition def = definitions_by_string[name];
            return def;
        }

        private SimConnect ConnectSimConnect()
        {
            try
            {
                // The constructor is similar to SimConnect_Open in the native API
                Console.Write("conneting to sim ");
                simconnect = new SimConnect("Simconnect - Simvar test", m_hWnd, WM_USER_SIMCONNECT, null, 0);
                simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(InitSimConnect);
                simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(OnRecvQuit);
                simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(OnRecvException);
                simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(OnRecvSimobjectDataBytype);
                simconnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(OnRecvSimobjectData);
                simconnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(simconnect_OnRecvEvent);

                foreach (EVENTS entry in Enum.GetValues(typeof(EVENTS)))
                {

                    if (Array.IndexOf(SystemEvents, entry) >= 0)
                    {
                        //Console.WriteLine($"sys: {entry}");
                        simconnect.SubscribeToSystemEvent(entry, entry.ToString());
                    }
                    else
                    {
                        //Console.WriteLine($"client: {entry}");
                        simconnect.MapClientEventToSimEvent(entry, entry.ToString());
                        simconnect.AddClientEventToNotificationGroup(GROUP_ID.GROUP_A, entry, false);
                    }


                }

                // set Group Priority
                simconnect.SetNotificationGroupPriority(GROUP_ID.GROUP_A, SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST);
                // simconnect.SetNotificationGroupPriority(GROUP_ID.GROUP_B, SimConnect.SIMCONNECT_GROUP_PRIORITY_HIGHEST);

                IsSimConnected = true;
                return simconnect;
            }
            catch
            {
                Console.WriteLine("[ERROR]: Unable to connect, Check if MSFS is running!");
                simconnect = null;
                IsSimConnected = false;
                return null;
            }
        }

        public override bool Update()
        {
            if (simconnect != null)
            {
                simconnect.ReceiveMessage();
                simconnect.ReceiveDispatch(new SignalProcDelegate(MyDispatchProcA));
                foreach (int i in definitions.Keys)
                {
                    // simconnect.RequestDataOnSimObjectType(definitions[(DATA_DEFINE_ID)i].reqId, definitions[(DATA_DEFINE_ID)i].defId, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);

                    simconnect.RequestDataOnSimObject(definitions[(DATA_DEFINE_ID)i].reqId, definitions[(DATA_DEFINE_ID)i].defId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SIM_FRAME, SIMCONNECT_DATA_REQUEST_FLAG.DEFAULT, 0, this.conf.SimconnectFrames, 0);
                }
                return true;
            }
            else
            {
                return ConnectSimConnect() == null;
            }
        }

        void simconnect_OnRecvEvent(SimConnect sender, SIMCONNECT_RECV_EVENT recEvent)
        {
            EVENTS ReceivedEvent = (EVENTS)recEvent.uEventID;
            // Console.WriteLine(ReceivedEvent);

            OnEvent(ReceivedEvent);
        }

        private void MyDispatchProcA(SIMCONNECT_RECV pData, uint cData)
        {
            // Console.WriteLine("MyDispatchProcA "+pData+" "+cData);
        }

        public override bool setDoubleValue(string name, double value)
        {
            // disable any write funktion until bug is fixed
            //return;
            try
            {
                DataDefinition def = getDefinitionByName(name);
                simconnect.SetDataOnSimObject(def.defId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, value);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("SimConnect setValue error: " + e.Message);
                return false;
                // throw e;
            }
        }

        public override bool setBoolValue(string name, bool value)
        {
            // disable any write funktion until bug is fixed
            //return;
            try
            {
                DataDefinition def = getDefinitionByName(name);
                simconnect.SetDataOnSimObject(def.defId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, value);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("SimConnect setValue error: " + e.Message);
                return false;
                // throw e;
            }
        }

        public override bool setIntValue(string name, int value)
        {
            // disable any write funktion until bug is fixed
            //return;
            try
            {
                DataDefinition def = getDefinitionByName(name);
                simconnect.SetDataOnSimObject(def.defId, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, value);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("SimConnect setValue error: " + e.Message);
                return false;
                // throw e;
            }
        }

        public override void Pause()
        {
            this.sendEvent(EVENTS.PAUSE_SET, 1);
            // this.sendEvent(EVENTS.PAUSE_ON, 1);
        }

        public override void Unpause()
        {
            this.sendEvent(EVENTS.PAUSE_SET, 0);
            // this.sendEvent(EVENTS.PAUSE_OFF, 1);
        }

        public override void sendEvent(EVENTS myEvent, uint dwData = 1)
        {
            try
            {
                simconnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER, myEvent, dwData, GROUP_ID.GROUP_A, SIMCONNECT_EVENT_FLAG.DEFAULT);
            }
            catch (Exception e)
            {
                Console.WriteLine("SimConnect sendEvent error: " + e.Message);
            }
        }


        public override void SpawnObject(string objectName, double latitude, double longitude, double altitude)
        {
            if (simconnect != null)
            {
                SIMCONNECT_DATA_INITPOSITION initPos = new SIMCONNECT_DATA_INITPOSITION
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = altitude,
                    Pitch = 0.0f,
                    Bank = 0.0f,
                    Heading = 0.0f,
                    OnGround = 1, // 1 = spawn on ground, 0 = spawn in air
                    Airspeed = 0
                };

                simconnect.AICreateSimulatedObject(objectName, initPos, REQUEST_ID.SPAWN_OBJECT);
                Console.WriteLine($"Spawning {objectName} at {latitude}, {longitude}, {altitude}");
            }
        }


        private void OnRecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            DataDefinition def = definitions[(DATA_DEFINE_ID)data.dwDefineID];
            if (def.isString)
            {
                DataStruct result = (DataStruct)data.dwData[0];
                // Console.WriteLine("SimConnect " + def.dname + " value: " + result.sValue);
                switch (def.dname)
                {
                    case "ATC MODEL":
                        {
                            Model = result.sValue;
                            break;
                        }
                    case "ATC TYPE":
                        {
                            Type = result.sValue;
                            break;
                        }
                    case "TITLE":
                        {
                            Title = result.sValue;
                            break;
                        }
                    case "NAV_LOC_AIRPORT_IDENT":
                        {
                            Airport = result.sValue;
                            break;
                        }
                }
            }
            else
            {
                // Console.WriteLine("SimConnect " + def.dname + " value: " + data.dwData[0]);
                switch (def.dname)
                {
                    case "PLANE ALTITUDE":
                        {
                            Altitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE LATITUDE":
                        {
                            Latitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE LONGITUDE":
                        {
                            Longitude = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE HEADING DEGREES TRUE":
                        {
                            Heading = (double)data.dwData[0];
                            break;
                        }
                    case "GROUND VELOCITY":
                        {
                            GroundSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE ALT ABOVE GROUND MINUS CG":
                        {
                            AltitudeAGL = (double)data.dwData[0];
                            break;
                        }

                    case "VERTICAL SPEED":
                        {
                            VerticalSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "AIRSPEED INDICATED":
                        {
                            Airspeed = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE PITCH DEGREES":
                        {
                            pitch = (double)data.dwData[0];
                            break;
                        }
                    case "PLANE BANK DEGREES":
                        {
                            bank = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY WORLD X":
                        {
                            vX = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY WORLD Y":
                        {
                            vY = (double)data.dwData[0];
                            break;
                        }
                    case "VELOCITY WORLD Z":
                        {
                            vZ = (double)data.dwData[0];
                            break;
                        }
                    case "G FORCE":
                        {
                            gforce = (double)data.dwData[0];
                            break;
                        }
                    case "ON ANY RUNWAY":
                        {
                            IsOnRundway = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "SIM ON GROUND":
                        {
                            IsOnGround = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "ENG COMBUSTION:1":
                        {
                            IsEngineOn = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "SIM DISABLED":
                        {
                            SimDisabled = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "BRAKE PARKING POSITION":
                        {
                            IsParkingBreak = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "FUEL TOTAL QUANTITY WEIGHT":
                        {
                            Fuel = (double)data.dwData[0];
                            break;
                        }
                    case "FUEL SELECTED QUANTITY PERCENT":
                        {
                            FuelPercent = (double)data.dwData[0] * 100;
                            break;
                        }
                    case "UNLIMITED FUEL":
                        {
                            FuelUnlimited = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "REALISM CRASH DETECTION":
                        {
                            CrashEnabled = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "IS SLEW ACTIVE":
                        {
                            IsSlew = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "PAYLOAD STATION WEIGHT:1":
                        {
                            PilotWeight = (double)data.dwData[0];
                            break;
                        }
                    case "TOTAL WEIGHT":
                        {
                            TotalWeight = (double)data.dwData[0];
                            break;
                        }
                    case "MAX GROSS WEIGHT":
                        {
                            MaxTotalWeight = (double)data.dwData[0];
                            break;
                        }
                    case "RIGHT WHEEL RPM":
                        {
                            RWheelRPM = (double)data.dwData[0];
                            break;
                        }
                    case "LEFT WHEEL RPM":
                        {
                            LWheelRPM = (double)data.dwData[0];
                            break;
                        }
                    case "CENTER WHEEL RPM":
                        {
                            CWheelRPM = (double)data.dwData[0];
                            break;
                        }
                    case "AUX WHEEL RPM":
                        {
                            AuxWheelRPM = (double)data.dwData[0];
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:0":
                        {
                            ContactPoints[0] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:1":
                        {
                            ContactPoints[1] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:2":
                        {
                            ContactPoints[2] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:3":
                        {
                            ContactPoints[3] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:4":
                        {
                            ContactPoints[4] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:5":
                        {
                            // potential Prop
                            ContactPoints[5] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:6":
                        {
                            // potential Prop
                            ContactPoints[6] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:7":
                        {
                            // potential Prop
                            ContactPoints[7] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:8":
                        {
                            // potential Prop
                            ContactPoints[8] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:9":
                        {
                            // potential Prop
                            ContactPoints[9] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:10":
                        {
                            // potential Prop
                            ContactPoints[10] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:11":
                        {
                            // potential Prop
                            ContactPoints[11] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:12":
                        {
                            // potential Prop
                            ContactPoints[12] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:13":
                        {
                            // potential Prop
                            ContactPoints[13] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:14":
                        {
                            // potential Prop
                            ContactPoints[14] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:15":
                        {
                            // potential Prop
                            ContactPoints[15] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:16":
                        {
                            // potential Prop
                            ContactPoints[16] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:17":
                        {
                            // potential Prop
                            ContactPoints[17] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:18":
                        {
                            // potential Prop
                            ContactPoints[18] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:19":
                        {
                            // potential Prop
                            ContactPoints[19] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "CONTACT POINT IS ON GROUND:20":
                        {
                            // potential Prop
                            ContactPoints[20] = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "AIRCRAFT WIND X":
                        {
                            WindX = (double)data.dwData[0];
                            break;
                        }
                    case "AIRCRAFT WIND Z":
                        {
                            WindY = (double)data.dwData[0];
                            break;
                        }
                    case "AMBIENT WIND DIRECTION":
                        {
                            AmbientWindDirection = (double)data.dwData[0];
                            break;
                        }
                    case "AMBIENT WIND VELOCITY":
                        {
                            AmbientWindSpeed = (double)data.dwData[0];
                            break;
                        }
                    case "FLAP SPEED EXCEEDED":
                        {
                            IsFlapsOverspeed = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "OVERSPEED WARNING":
                        {
                            IsVNEOverspeed = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "AI ANTISTALL STATE":
                        {
                            Antistall = (double)data.dwData[0];
                            break;
                        }
                    case "AI AUTOTRIM ACTIVE":
                        {
                            Autotrim = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "AI CONTROLS":
                        {
                            AICtrl = (double)data.dwData[0] > 0;
                            break;
                        }
                    case "FLAPS HANDLE PERCENT":
                        {
                            FlapsPercent = (double)data.dwData[0];
                            break;
                        }
                    case "FLAPS HANDLE INDEX":
                        {
                            FlapsIndex = (uint)Math.Floor((double)data.dwData[0]);
                            break;
                        }
                    case "AILERON POSITION":
                        {
                            AileronPosition = (double)data.dwData[0];
                            break;
                        }
                    case "ELEVATOR POSITION":
                        {
                            ElevatorPosition = (double)data.dwData[0];
                            break;
                        }
                    case "RUDDER POSITION":
                        {
                            RudderPosition = (double)data.dwData[0];
                            break;
                        }
                    case "GENERAL ENG THROTTLE LEVER POSITION:1":
                        {
                            ThrottlePosition = (double)data.dwData[0];
                            break;
                        }
                    case "PROP RPM:1":
                        {
                            PropRPM = (double)data.dwData[0];
                            break;
                        }
                    case "AMBIENT TEMPERATURE":
                        {
                            TemperatureAmbient = (double)data.dwData[0];
                            break;
                        }
                    case "AMBIENT PRESSURE":
                        {
                            PressureAmbient = (double)data.dwData[0];
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            SIMCONNECT_RECV_SIMOBJECT_DATA data2 = (SIMCONNECT_RECV_SIMOBJECT_DATA)data;
            OnRecvSimobjectData(sender, data2);
            return;
        }

        private void OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            Console.WriteLine("SimConnect exception: " + data.dwException + " " + data.dwIndex);
        }

        private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            this.Unpause();
            simconnect.Dispose();
            simconnect = null;
            IsSimConnected = false;
            SimDisabled = false;
            Console.WriteLine("SimConnect quit");
            this.OnQuit();
        }

    }
}
