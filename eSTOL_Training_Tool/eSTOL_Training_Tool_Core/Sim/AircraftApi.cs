using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using STOL_Training_Tool_Core.Core;
using Newtonsoft.Json;

namespace STOL_Training_Tool
{
    public class AircraftApi : Plane
    {
        private readonly HttpListener? listener;
        private readonly CancellationTokenSource cts = new();
        private readonly Dictionary<(string path, string method), Func<HttpListenerContext, Task>> routes;
        private readonly int port;
        private readonly string host;
        

        public AircraftApi(PlaneEventCallBack callback, string host = "", int port = -1) : base(callback)
        {
            this.isReadonly = true;
            this.conf = Config.GetInstance();

            this.port = port == -1 ? conf.ApiPort : port;
            this.host = string.IsNullOrWhiteSpace(host) ? conf.ApiHost : host;

            // --- ROUTES ---
            routes = new Dictionary<(string, string), Func<HttpListenerContext, Task>>()
            {
                { ("/telemetry", "POST"), HandleTelemetryAsync },
                { ("/plane", "POST"), HandlePlaneAsync },
                { ("/connect", "POST"), HandleConnectAsync },
                { ("/", "GET"), HandleRootAsync }
            };

            // --- START LISTENER ---
            listener = new HttpListener();
            listener.Prefixes.Add($"http://{this.host}:{this.port}/");

            try
            {
                listener.Start();
                _ = Task.Run(() => ListenLoopAsync(cts.Token));
            }
            catch (Exception)
            {
                listener = null; // could not start listener
            }
        }

        private async Task ListenLoopAsync(CancellationToken token)
        {
            if (listener == null) return;

            while (!token.IsCancellationRequested)
            {
                HttpListenerContext ctx = null;
                try
                {
                    ctx = await listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    break; // listener stopped or canceled
                }

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await HandleContextAsync(ctx).ConfigureAwait(false);
                    }
                    catch
                    {
                        TryWrite(ctx, "Internal Server Error", 500);
                    }
                }, token);
            }
        }

        private async Task HandleContextAsync(HttpListenerContext ctx)
        {
            string path = ctx.Request.Url?.AbsolutePath ?? "/";

            // Handle CORS preflight
            if (ctx.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                ctx.Response.AddHeader("Access-Control-Allow-Origin", "*"); // or your sender address
                ctx.Response.AddHeader("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                ctx.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
                ctx.Response.StatusCode = 200;
                ctx.Response.Close();
                return;
            }

            // Add CORS headers for all other responses
            ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");

            // Existing POST /telemetry, POST /plane, etc...

            string method = ctx.Request.HttpMethod.ToUpperInvariant();
            if (routes.TryGetValue((path, method), out var handler))
            {
                await handler(ctx).ConfigureAwait(false);
            }
            else
            {
                await TryWrite(ctx, "Not found", 404);
            }
            
        }

        // === HANDLERS ===

        private async Task HandleTelemetryAsync(HttpListenerContext ctx)
        {
            string json = await ReadBodyAsync(ctx);
            try
            {
                Telemetrie telem = JsonConvert.DeserializeObject<Telemetrie>(json) ?? new Telemetrie();

                if (telem.PositionCG != null)
                {
                    Latitude = telem.Position.Latitude;
                    Longitude = telem.Position.Longitude;
                }

                Altitude = telem.Altitude;
                AltitudeAGL = telem.AltitudeAGL;
                Height_AGL = telem.Height;
                GroundSpeed = telem.GroundSpeed;
                Airspeed = telem.AirSpeed;
                Heading = telem.Heading;
                vX = telem.vX;
                vY = telem.vY;
                vZ = telem.vZ;
                pitch = telem.pitch;
                bank = telem.bank;
                VerticalSpeed = telem.verticalSpeed;
                gforce = telem.gForce;
                FlapsPercent = telem.FlapsPercent;
                FlapsIndex = telem.FlapsHandlePosition;
                AileronPosition = telem.AileronsPercent;
                ElevatorPosition = telem.ElevatorsPercent;
                RudderPosition = telem.RudderPercent;
                ThrottlePosition = telem.ThrottlePosition;
                IsOnGround = telem.OnGround;

                ContactPoints = telem.ContactPoints;

                WindX = telem.WindX;
                WindY = telem.WindY;

                isInit = true;
                IsSimConnected = true;

                await TryWrite(ctx, "OK");
            }
            catch (JsonException)
            {
                await TryWrite(ctx, "Invalid JSON", 400);
            }
        }

        private async Task HandlePlaneAsync(HttpListenerContext ctx)
        {
            string json = await ReadBodyAsync(ctx);
            try
            {
                var planeData = JsonConvert.DeserializeObject<PlaneMetadata>(json);

                if (planeData != null)
                {
                    Title = planeData.Title;
                    Type = planeData.Type;
                    Model = planeData.Model;
                }

                await TryWrite(ctx, "OK");
            }
            catch (Exception ex)
            {
                await TryWrite(ctx, "Error: " + ex.Message, 500);
            }
        }

        private async Task HandleConnectAsync(HttpListenerContext ctx)
        {
            await TryWrite(ctx, "OK");
        }

        private async Task HandleRootAsync(HttpListenerContext ctx)
        {
            string info = $"STOL REST API running on {host}:{port}";
            await TryWrite(ctx, info);
        }

        // === HELPERS ===

        private static async Task<string> ReadBodyAsync(HttpListenerContext ctx)
        {
            using var sr = new System.IO.StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding);
            return await sr.ReadToEndAsync().ConfigureAwait(false);
        }

        private static async Task TryWrite(HttpListenerContext ctx, string message, int status = 200)
        {
            try
            {
                byte[] resp = Encoding.UTF8.GetBytes(message);
                ctx.Response.StatusCode = status;
                ctx.Response.ContentType = "text/plain; charset=utf-8";
                ctx.Response.ContentLength64 = resp.Length;
                await ctx.Response.OutputStream.WriteAsync(resp, 0, resp.Length).ConfigureAwait(false);
            }
            catch { }
            finally
            {
                try { ctx.Response.Close(); } catch { }
            }
        }

        // === BASE CLASS OVERRIDES ===

        public void StopListener()
        {
            try
            {
                cts.Cancel();
                listener?.Close();
            }
            catch { }
        }

        public override void Pause() { }
        public override void sendEvent(EVENTS myEvent, uint dwData = 1) { }
        public override bool setDoubleValue(string name, double value) { return false; }
        public override bool setBoolValue(string name, bool value) { return false; }
        public override bool setIntValue(string name, int value) { return false; }
        public override void SpawnObject(string objectName, double latitude, double longitude, double altitude) { }
        public override void Unpause() { }

        public override bool Update() => isInit;
    }
}
