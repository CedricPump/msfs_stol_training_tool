using System;
using STOL_Training_Tool.Model;

namespace STOL_Training_Tool
{
    public abstract class Plane : Aircraft
    {
        public delegate void PlaneEventCallBack(PlaneEvent planeEvent);
        private PlaneEventCallBack callBack = null;

        public bool isReadonly = false;

        public Plane(PlaneEventCallBack callback): base()
        {
            this.callBack = callback;
        }

        public override void OnEvent(EVENTS recEvent)
        {
            switch (recEvent)
            {
                default:
                    {
                           
                        this.callBack(new PlaneEvent
                        {
                            Event = recEvent.ToString(),
                            Parameter = new InitFlightData
                            {
                                Ident = this.GetIdent(),
                                State = this.GetState(),
                                Telemetrie = this.GetTelemetrie()
                            }
                        });
                            
                        break;
                    }
                case EVENTS.SimStop:
                    {
                        this.callBack(new PlaneEvent
                        {
                            Event = EVENTS.SimStop.ToString(),
                            Parameter = new object[0]
                        });
                        break;
                    }

                case EVENTS.PAUSE_TOGGLE:
                case EVENTS.PAUSE_ON:
                case EVENTS.PAUSE_OFF:
                    {
                        this.callBack(new PlaneEvent
                        {
                            Event = EVENTS.PAUSE_TOGGLE.ToString(),
                            Parameter = new object[0]
                        });
                        break;
                    }
            }
        }

        public override void OnQuit()
        {
            this.callBack(new PlaneEvent
            {
                Event = "QUIT",
                Parameter = new object[0]
            });
            //System.Environment.Exit(0);
        }

        public void ResetConfig()
        {
            this.ConfigKey = null;
            this.GetPlaneConfig();
        }
    }
    
}
