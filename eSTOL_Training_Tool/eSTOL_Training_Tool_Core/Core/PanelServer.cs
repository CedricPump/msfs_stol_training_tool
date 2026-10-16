using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace STOL_Training_Tool_Core.Core
{
    public class PanelAlignedStatus
    {
        public string Text { get; set; } = "";
        public string Color { get; set; } = "#808080";
    }

    public class PanelWindStatus
    {
        public double SpeedKt { get; set; } = 0;
        public double RelativeDirDeg { get; set; } = 0;
    }

    public class PanelRemark
    {
        public string Type { get; set; } = "";

        /// <summary>0=Remark, 1=Warning, 2=Deviation, 3=Violation - same scale
        /// and colors (LightGray/Yellow/Orange/Red) as FormUI's deviations
        /// list on the desktop app.</summary>
        public int Severity { get; set; } = 0;
    }

    public class PanelStatus
    {
        public bool Connected { get; set; } = false;
        public string State { get; set; } = "Unknown";
        public PanelAlignedStatus Aligned { get; set; } = new PanelAlignedStatus();
        public bool HasTakeoff { get; set; } = false;
        public double TakeoffDistance { get; set; } = 0;
        public bool HasTouchdown { get; set; } = false;
        public double TouchdownDistance { get; set; } = 0;
        public double LandingRateFpm { get; set; } = 0;
        public bool HasLanding { get; set; } = false;
        public double StoppingDistance { get; set; } = 0;
        public double LandingDistance { get; set; } = 0;
        public double Score { get; set; } = 0;
        public bool IsScratch { get; set; } = false;
        public bool IsPropStrike { get; set; } = false;
        public List<PanelRemark> Remarks { get; set; } = new List<PanelRemark>();
        public string Unit { get; set; } = "feet";
        public double AglFt { get; set; } = 0;
        public bool TimerRunning { get; set; } = false;
        public double ElapsedSeconds { get; set; } = 0;
        public PanelWindStatus Wind { get; set; } = new PanelWindStatus();
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool TestMode { get; set; } = false;
    }

    /// <summary>
    /// Lightweight local HTTP server that exposes the current STOL status as JSON
    /// for the in-sim MSFS toolbar panel (html_ui InGamePanel) to poll via fetch().
    /// Runs independent of the SimConnect/REST connection type used for input.
    ///
    /// Also hosts a "/test" control page (dev-only, not shipped in the MSFS
    /// Community package) that lets you fake takeoff/touchdown/stopping/wind/etc.
    /// scenarios without flying, by overriding what "/status" serves. Flipping
    /// test mode off always instantly restores real live data - the real
    /// Controller loop keeps writing to `status` unconditionally regardless of
    /// test mode, it's simply not what gets served while test mode is on.
    /// </summary>
    public class PanelServer
    {
        private HttpListener listener;
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly object statusLock = new object();
        private readonly PanelStatus status = new PanelStatus();

        private readonly object testLock = new object();
        private bool testModeEnabled = false;
        private PanelStatus testStatus = new PanelStatus();
        private int alignedCycleIndex = 0;

        private static readonly (string Text, string Color)[] AlignedCycleStates = new[]
        {
            ("Sim not connected", "#FF0000"),
            ("Sim not initialized", "#FF8C00"),
            ("aligned (12 ft)", "#90EE90"),
            ("aligned (bad heading,12 ft)", "#90EE90"),
            ("on lineup (150 ft)", "#FFFFE0"),
            ("down field", "#FFFFE0"),
            ("NOT ALIGNED", "#CD5C5C"),
            ("No Reference: Apply Preset", "#FF0000"),
            ("", "#F0F0F0"), // airborne / blank state
        };

        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public bool IsRunning => listener != null && listener.IsListening;
        public string Host { get; }
        public int Port { get; }

        public PanelServer(string host, int port)
        {
            Host = host;
            Port = port;

            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add($"http://{host}:{port}/");
                listener.Start();
                _ = Task.Run(() => ListenLoopAsync(cts.Token));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PanelServer] Failed to start on {host}:{port}: {ex.Message}");
                listener = null;
            }
        }

        public void UpdateStatus(Action<PanelStatus> mutate)
        {
            lock (statusLock)
            {
                mutate(status);
                status.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        private string GetEffectiveStatusJson()
        {
            lock (testLock)
            {
                if (testModeEnabled)
                {
                    testStatus.TestMode = true;
                    return JsonSerializer.Serialize(testStatus, jsonOptions);
                }
            }

            lock (statusLock)
            {
                status.TestMode = false;
                return JsonSerializer.Serialize(status, jsonOptions);
            }
        }

        private async Task ListenLoopAsync(CancellationToken token)
        {
            if (listener == null) return;

            while (!token.IsCancellationRequested)
            {
                HttpListenerContext ctx;
                try
                {
                    ctx = await listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    break; // listener stopped/disposed
                }

                _ = Task.Run(() => HandleContextAsync(ctx), token);
            }
        }

        private async Task HandleContextAsync(HttpListenerContext ctx)
        {
            try
            {
                ctx.Response.AddHeader("Access-Control-Allow-Origin", "*");

                if (ctx.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    ctx.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
                    ctx.Response.StatusCode = 200;
                    ctx.Response.Close();
                    return;
                }

                string path = ctx.Request.Url?.AbsolutePath ?? "/";
                string method = ctx.Request.HttpMethod.ToUpperInvariant();

                if (path == "/status" && method == "GET")
                {
                    await WriteAsync(ctx, GetEffectiveStatusJson(), "application/json").ConfigureAwait(false);
                }
                else if (path == "/test" && method == "GET")
                {
                    await WriteAsync(ctx, TestPageHtml, "text/html").ConfigureAwait(false);
                }
                else if (path == "/test/action" && method == "POST")
                {
                    string body = await ReadBodyAsync(ctx).ConfigureAwait(false);
                    string type = ExtractJsonStringField(body, "type") ?? "";
                    HandleTestAction(type);
                    await WriteAsync(ctx, GetEffectiveStatusJson(), "application/json").ConfigureAwait(false);
                }
                else if (path == "/" && method == "GET")
                {
                    await WriteAsync(ctx, $"STOL Panel Status Server running on {Host}:{Port}", "text/plain").ConfigureAwait(false);
                }
                else
                {
                    await WriteAsync(ctx, "Not found", "text/plain", 404).ConfigureAwait(false);
                }
            }
            catch
            {
                try { ctx.Response.Close(); } catch { }
            }
        }

        private void HandleTestAction(string type)
        {
            lock (testLock)
            {
                switch (type)
                {
                    case "enable":
                        testModeEnabled = true;
                        testStatus = new PanelStatus
                        {
                            Connected = true,
                            State = "Hold",
                            Aligned = new PanelAlignedStatus { Text = "TEST MODE", Color = "#808080" }
                        };
                        alignedCycleIndex = 0;
                        break;

                    case "disable":
                        testModeEnabled = false;
                        break;

                    case "reset":
                        testStatus = new PanelStatus
                        {
                            Connected = true,
                            State = "Hold",
                            Aligned = new PanelAlignedStatus { Text = "TEST MODE", Color = "#808080" }
                        };
                        alignedCycleIndex = 0;
                        break;

                    case "new_run":
                        testStatus.State = "Takeoff";
                        testStatus.HasTakeoff = true;
                        testStatus.TakeoffDistance = Random.Shared.Next(50, 401);
                        testStatus.Unit = "feet";
                        // Mirrors ResetPanelRunStatus() in Controller.cs.
                        testStatus.HasTouchdown = false;
                        testStatus.TouchdownDistance = 0;
                        testStatus.LandingRateFpm = 0;
                        testStatus.HasLanding = false;
                        testStatus.StoppingDistance = 0;
                        testStatus.LandingDistance = 0;
                        testStatus.Score = 0;
                        testStatus.IsScratch = false;
                        testStatus.IsPropStrike = false;
                        testStatus.Remarks = new List<PanelRemark>();
                        testStatus.TimerRunning = true;
                        testStatus.ElapsedSeconds = 0;
                        break;

                    case "touchdown":
                        {
                            double touchdown = Random.Shared.Next(-50, 401);
                            testStatus.HasTouchdown = true;
                            testStatus.TouchdownDistance = touchdown;
                            testStatus.LandingRateFpm = Random.Shared.Next(-600, -49);
                            testStatus.IsScratch = touchdown <= 0;
                            testStatus.State = "Rollout";
                            break;
                        }

                    case "stopping":
                        {
                            double stopping = Random.Shared.Next(50, 601);
                            testStatus.HasLanding = true;
                            testStatus.StoppingDistance = stopping;
                            testStatus.LandingDistance = testStatus.TouchdownDistance + stopping;
                            testStatus.Score = testStatus.IsScratch ? 0 : (testStatus.TakeoffDistance + testStatus.LandingDistance);
                            testStatus.State = "Hold";
                            testStatus.TimerRunning = false;
                            testStatus.Remarks = testStatus.IsScratch
                                ? new List<PanelRemark> { new PanelRemark { Type = "TouchdownLineViolation", Severity = 3 } }
                                : new List<PanelRemark>
                                {
                                    new PanelRemark { Type = "ExcessiveGs", Severity = 2 },
                                    new PanelRemark { Type = "ParkingBrake", Severity = 1 },
                                    new PanelRemark { Type = "TouchNGo", Severity = 0 },
                                };
                            break;
                        }

                    case "prop_strike":
                        testStatus.IsPropStrike = true;
                        break;

                    case "wind":
                        testStatus.Wind.SpeedKt = Math.Round(Random.Shared.NextDouble() * 25, 1);
                        testStatus.Wind.RelativeDirDeg = Math.Round(Random.Shared.NextDouble() * 360);
                        break;

                    case "agl":
                        testStatus.AglFt = Random.Shared.Next(0, 2001);
                        break;

                    case "cycle_aligned":
                        alignedCycleIndex = (alignedCycleIndex + 1) % AlignedCycleStates.Length;
                        var next = AlignedCycleStates[alignedCycleIndex];
                        testStatus.Aligned.Text = next.Text;
                        testStatus.Aligned.Color = next.Color;
                        break;

                    case "toggle_timer":
                        testStatus.TimerRunning = !testStatus.TimerRunning;
                        break;

                    case "toggle_connected":
                        testStatus.Connected = !testStatus.Connected;
                        break;
                }

                testStatus.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        private static async Task<string> ReadBodyAsync(HttpListenerContext ctx)
        {
            using var reader = new StreamReader(ctx.Request.InputStream, ctx.Request.ContentEncoding ?? Encoding.UTF8);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        private static string ExtractJsonStringField(string json, string field)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty(field, out var value))
                {
                    return value.GetString();
                }
            }
            catch { }
            return null;
        }

        private static async Task WriteAsync(HttpListenerContext ctx, string body, string contentType, int statusCode = 200)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                ctx.Response.StatusCode = statusCode;
                ctx.Response.ContentType = $"{contentType}; charset=utf-8";
                ctx.Response.ContentLength64 = bytes.Length;
                await ctx.Response.OutputStream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
            catch { }
            finally
            {
                try { ctx.Response.Close(); } catch { }
            }
        }

        public void Stop()
        {
            try
            {
                cts.Cancel();
                listener?.Stop();
                listener?.Close();
            }
            catch { }
        }

        private const string TestPageHtml = @"<!DOCTYPE html>
<html>
<head>
<meta charset=""utf-8"" />
<title>STOL Panel Test Mode</title>
<style>
  body { font-family: Segoe UI, sans-serif; background: #14181c; color: #f0f0f0; padding: 16px; }
  h1 { font-size: 18px; }
  .row { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 12px; }
  button { background: #2a2e38; color: #e8e8ec; border: 1px solid #444a58; border-radius: 4px; padding: 8px 12px; cursor: pointer; font-size: 13px; }
  button:hover { background: #3a3f4c; }
  button.master { background: #2f8a5c; border-color: #43c07a; }
  button.danger { background: #8a2f2f; border-color: #c04343; }
  pre { background: #1c1f26; padding: 12px; border-radius: 6px; overflow: auto; max-height: 400px; }
  #modeLabel { font-weight: 700; }
</style>
</head>
<body>
<h1>STOL Panel Test Mode</h1>
<p>Test mode: <span id=""modeLabel"">unknown</span> - open the real in-sim panel (or its panel.html directly) in another window to watch it react live.</p>

<div class=""row"">
  <button class=""master"" onclick=""send('enable')"">Enable Test Mode</button>
  <button class=""danger"" onclick=""send('disable')"">Disable Test Mode</button>
  <button onclick=""send('reset')"">Reset</button>
</div>

<div class=""row"">
  <button onclick=""send('new_run')"">New Run (random takeoff)</button>
  <button onclick=""send('touchdown')"">Random Touchdown</button>
  <button onclick=""send('stopping')"">Random Stopping</button>
  <button class=""danger"" onclick=""send('prop_strike')"">Prop Strike</button>
</div>

<div class=""row"">
  <button onclick=""send('wind')"">Randomize Wind</button>
  <button onclick=""send('agl')"">Randomize AGL</button>
  <button onclick=""send('cycle_aligned')"">Cycle Aligned State</button>
  <button onclick=""send('toggle_timer')"">Toggle Timer</button>
  <button onclick=""send('toggle_connected')"">Toggle Connected</button>
</div>

<pre id=""out"">(no data yet)</pre>

<script>
function send(type) {
  fetch('/test/action', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ type: type })
  }).then(function (r) { return r.json(); }).then(render);
}
function poll() {
  fetch('/status', { cache: 'no-store' }).then(function (r) { return r.json(); }).then(render).catch(function () {});
  setTimeout(poll, 1000);
}
function render(data) {
  document.getElementById('modeLabel').textContent = data.testMode ? 'ON' : 'off (live data)';
  document.getElementById('out').textContent = JSON.stringify(data, null, 2);
}
poll();
</script>
</body>
</html>";
    }
}
