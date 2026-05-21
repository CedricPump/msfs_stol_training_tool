using System;
using System.Device.Location;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using STOL_Training_Tool;
using STOL_Training_Tool_Core.Core;
using STOL_Training_Tool_Core.Model;


namespace STOL_Training_Tool_Core.UI
{
    public partial class FormUI : Form
    {
        delegate void SetResultTextCallback(string text);
        delegate void SetStatusTextCallback(string text);
        private readonly Controller controller;
        private STOLResult? result = null;
        private TimeSpan StopwatchOffset = TimeSpan.Zero;
        System.Diagnostics.Stopwatch stopwatch;
        private bool alwaysontop = false;
        private int stopwatchOffsetSeconds = 30;
        private bool debug = false;
        private object drawing = new object();

        // stol ref
        private GeoCoordinate InitailPos = null;
        private GeoCoordinate PlanePos = null;
        private double InitialHeading = 0.0;

        private double WindDir = 0.0;
        public double Wind = 0.0;

        private int fieldSizeFull = 600;
        private int fieldSizeZoom = 220;
        private int selectedFieldSize = 600;

        private string aligned = "";
        private Color alignColor = SystemColors.Control;

        private static readonly Color myDarkControl = Color.FromArgb(0x20, 0x20, 0x20);
        private static readonly Color myDarkControlText = Color.LightGray;

        private int deviations = 0;

        private Config config = Config.GetInstance();

        public FormUI(Controller controller)
        {
            InitializeComponent();

#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            Color myForeColor = SystemColors.ControlText;
            Color myBackColor = SystemColors.Control;
            if (Application.IsDarkModeEnabled)
            {
                myBackColor = myDarkControl;
                myForeColor = myDarkControlText;

                this.textBoxResult.BackColor = myBackColor;
                this.textBoxStatus.BackColor = myBackColor;
                this.richTextBoxDeviations.BackColor = myBackColor;
                this.linkLabelRecordings.LinkColor = Color.FromArgb(0x60cdff);
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CA1416 // Validate platform compatibility


            if (config.transparencyPercent > 0)
            {
                // Clamp between 0 and 100
                int percent = Math.Max(0, Math.Min(90, config.transparencyPercent));

                // Convert percent to a value between 0.0 (fully transparent) and 1.0 (fully opaque)
                this.Opacity = 1 - percent / 100.0;
            }

            alwaysontop = config.alwaysOnTop;
            this.checkBoxOntop.Checked = alwaysontop;

            this.Text = "STOL Training Tool " + VersionHelper.GetVersion();

            this.controller = controller;
            textBoxUser.Text = controller.user;
            textBoxStatus.Text = "No Reference Position selected";
            buttonTeleport.Enabled = false;
            this.appendResult($"Welcome\r\n\r\nSelect a STOL field preset or \"Open World\" mode set custom start.");

            if (config.ConnectionType == "REST")
            {
                this.appendResult($"\r\nConnection Mode: REST API (InGamePanel)\r\nconnect to http://{config.ApiHost}:{config.ApiPort}");
            }

            comboBoxUnit.Items.Add("feet");
            comboBoxUnit.Items.Add("meters");
            comboBoxUnit.Items.Add("yard");
            comboBoxUnit.Text = config.Unit;
            // comboBoxUnit.Items.Add("yard");

            this.stopwatch = new System.Diagnostics.Stopwatch();
            this.progressBarStopwatch.Minimum = 0;
            this.progressBarStopwatch.Maximum = 180;

            this.checkBoxResult.Checked = config.isSendResults;
            this.checkBoxTelemetry.Checked = config.isSendTelemetry;
            this.checkBoxSaveRecording.Checked = config.enableGPXRecodering;

            this.numericUpDownStopwatchOffest.Value = stopwatchOffsetSeconds;

            this.checkBoxPropStrike.Checked = config.simulatePropStrike;

            new UILogger(this);
        }

        public void setPresets(string[] strings)
        {
            comboBoxPreset.Items.Clear();
            comboBoxPreset.Items.Add("Open World");
            comboBoxPreset.Items.AddRange(strings);
            comboBoxPreset.Text = "Open World";
        }

        public void setState(string stateStr)
        {
            if (this.textBoxStatus.InvokeRequired)
            {
                SetStatusTextCallback d = new SetStatusTextCallback(setState);
                this.Invoke(d, new object[] { stateStr });
                return;
            }
            textBoxStatus.Text = stateStr;
        }

        public void setResult(string resultStr)
        {

            if (this.textBoxResult.InvokeRequired)
            {
                SetResultTextCallback d = new SetResultTextCallback(setResult);
                this.Invoke(d, new object[] { resultStr });
                return;
            }
            textBoxResult.Text = resultStr;
        }

        public void appendResult(string text, bool autoscroll = true)
        {
            string newText = text + "\r\n";
            int limit = (int)Math.Min(config.ResultTextBoxCharacterLimit, int.MaxValue);

            Action appendAction = () =>
            {
                int currentLen = textBoxResult.TextLength;
                int excessAfterAppend = currentLen + newText.Length - limit;
                if (excessAfterAppend > 0 && currentLen > 0)
                {
                    int removeCount = Math.Min(excessAfterAppend, currentLen);

                    // Preserve selection/caret
                    int selStart = textBoxResult.SelectionStart;
                    int selLength = textBoxResult.SelectionLength;
                    bool hadFocus = textBoxResult.Focused;

                    // Remove from start by selecting and replacing with empty string
                    textBoxResult.SelectionStart = 0;
                    textBoxResult.SelectionLength = removeCount;
                    textBoxResult.SelectedText = string.Empty;

                    // Adjust selection start if it was after removed block
                    selStart = selStart <= removeCount ? 0 : selStart - removeCount;
                    textBoxResult.SelectionStart = selStart;
                    textBoxResult.SelectionLength = selLength;

                    if (hadFocus) textBoxResult.Focus();
                }

                // Append new text and optionally scroll
                textBoxResult.AppendText(newText);
                if (textBoxResult.Visible && autoscroll)
                {
                    textBoxResult.ScrollToCaret();
                }
            };

            if (this.textBoxResult.InvokeRequired)
            {
                this.textBoxResult.Invoke(appendAction);
                return;
            }

            appendAction();
        }

        private void buttonSetRefPos_Click(object sender, EventArgs e)
        {
            if (!controller.IsSimConnected())
            {
                MessageBox.Show("Sim not connected");
                return;
            }
            controller.SetStartPos();
            buttonTeleport.Enabled = controller.IsStolInit() && !controller.plane.isReadonly;
        }

        private void buttonApplyPreset_Click(object sender, EventArgs e)
        {
            if (!controller.IsSimConnected())
            {
                MessageBox.Show("Sim not connected");
                return;
            }

            string presetStr = comboBoxPreset.Text;
            controller.SetPreset(presetStr);
            if (presetStr != "Open World")
            {
                buttonTeleport.Enabled = !controller.plane.isReadonly;
                buttonSetRefPos.Enabled = false;
                this.appendResult($"Preset selected: {comboBoxPreset.Text}.\r\nTeleport to reference line?");
            }
            else
            {
                this.appendResult($"\"Open World\" Mode selected.");
                buttonTeleport.Enabled = controller.IsStolInit() && !controller.plane.isReadonly;
                buttonSetRefPos.Enabled = true;
            }
        }

        private void buttonTeleport_Click(object sender, EventArgs e)
        {
            if (!controller.IsSimConnected())
            {
                MessageBox.Show("Sim not connected");
                return;
            }

            DialogResult result = DialogResult.None;
            if (Config.GetInstance().showTelportConfirmation)
            {
                using var dialog = new TeleportDialog();
                result = dialog.ShowDialog();
                if (dialog.DontShowAgain)
                {
                    config.showTelportConfirmation = false;
                    config.Save();
                }
            }
            else
            {
                result = DialogResult.Yes;
            }

            if (result != DialogResult.Yes)
            {
                // Abort teleport if the user didn't click Yes
                return;
            }

            controller.TeleportToReferenceLine();
            // this.clearResultBox();
        }

        private void clearResultBox()
        {
            this.textBoxResult.Text = "";
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            // controller.SetUser(textBoxUser.Text);
        }

        private void textBoxUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string user = textBoxUser.Text;
                controller.SetUser(textBoxUser.Text);
                this.appendResult($"User {textBoxUser.Text} saved");

                if (user == "")
                {
                    var config = Config.GetInstance();
                    config.isSendResults = false;
                    this.checkBoxResult.Checked = false;
                    config.isSendTelemetry = false;
                    this.checkBoxTelemetry.Checked = false;
                    config.Save();
                }
            }
        }

        private void textBoxSessionKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                controller.SetSession(textBoxSessionKey.Text);
                this.appendResult($"Applied session key");
            }
        }

        private void buttonCreatePreset_Click(object sender, EventArgs e)
        {
            if (!controller.IsSimConnected())
            {
                MessageBox.Show("Sim not connected");
                return;
            }

            Preset preset = controller.createPresetFromCurrent();
            this.TopMost = false;

            try
            {

                using var dialog = new FormCreatePresset(preset, controller);
                dialog.ShowDialog();
                if (dialog.success)
                {
                    this.appendResult($"Preset \"{preset.title}\" created and saved to custom presets.");
                    // reload presets
                    controller.reloadPresets();
                }
            }
            finally
            {
                this.TopMost = alwaysontop;
            }
        }

        public void DrawResult(STOLResult result)
        {
            this.result = result;
            panel.Invalidate(); // Redraw the panel
            panel.BackColor = Color.FromArgb(0, 128, 0); // Dark Green
        }

        private void panel_Resize(object sender, EventArgs e)
        {
            panel.Invalidate(); // Triggers Paint event
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            lock (drawing)
            {
                Graphics g = e.Graphics;
                g.Clear(Color.Transparent); // Background green
                g.DrawImage(panel.BackgroundImage, new Rectangle(0, 0, this.Width, this.Height));

                // Canvas size
                int canvasWidth = e.ClipRectangle.Width;
                int canvasHeight = e.ClipRectangle.Height;

                // Logical runway size
                int logicalFieldWidth = 140;
                int logicalFieldLength = selectedFieldSize;

                // Independent scaling
                float scaleX = (float)canvasWidth / logicalFieldWidth;
                float scaleY = (float)canvasHeight / logicalFieldLength;

                // Pens
                using Pen whitePen = new Pen(Color.White, 1);
                using Pen redPen = new Pen(Color.Red, 2);
                using Pen bluePen = new Pen(Color.Blue, 3);
                using Pen orangePen = new Pen(Color.Orange, 3);
                using Pen yellowPen = new Pen(Color.Yellow, 3);
                using Pen blackPen = new Pen(Color.Black, 5);

                // Draw horizontal marker lines every 10feet
                for (int i = 10; i <= logicalFieldLength; i += 10)
                {
                    float y = canvasHeight - i * scaleY;
                    Pen pen = (i % 100 == 0) ? redPen : whitePen;
                    g.DrawLine(pen, 0, y, canvasWidth, y);
                }

                if (this.PlanePos != null)
                {
                    (double planeDist_y, double planeOffset_x) = GeoUtils.GetDistanceAlongAxis(this.InitailPos, this.PlanePos, this.InitialHeading);
                    float planeDist = (float)planeDist_y * scaleY * 3.28084f;
                    float planeOff = (float)planeOffset_x * scaleX * 3.28084f;

                    g.DrawEllipse(blackPen, canvasWidth / 2f + planeOff, canvasHeight - planeDist, 1, 1);
                }

                // Result-dependent lines
                if (result != null)
                {
                    // lets make the distance lines more precise
                    // result.InitialPosition; // start position on bottom field Line
                    // result.TakeoffPosition;
                    // result.TouchdownPosition;
                    // result.StopPosition;

                    (double toDist_y, double toOffset_x) = GeoUtils.GetDistanceAlongAxis(result.InitialPosition, result.TakeoffPosition, result.InitialHeading);
                    (double tdDist_y, double tdOffset_x) = GeoUtils.GetDistanceAlongAxis(result.InitialPosition, result.TouchdownPosition, result.InitialHeading);
                    (double stopDist_y, double stopOffset_x) = GeoUtils.GetDistanceAlongAxis(result.InitialPosition, result.StopPosition, result.InitialHeading);

                    float toDist = (float)toDist_y * scaleY * 3.28084f;
                    float toOff = (float)toOffset_x * scaleX * 3.28084f;

                    float tdDist = (float)tdDist_y * scaleY * 3.28084f;
                    float tdOff = (float)tdOffset_x * scaleX * 3.28084f;

                    float stopDist = (float)stopDist_y * scaleY * 3.28084f;
                    float stopOff = (float)stopOffset_x * scaleX * 3.28084f;

                    // Takeoff line
                    g.DrawLine(bluePen, canvasWidth / 2f + toOff, canvasHeight, canvasWidth / 2f + toOff, canvasHeight - toDist);
                    // Touchdown line
                    g.DrawLine(yellowPen, canvasWidth / 2f + tdOff, canvasHeight, canvasWidth / 2f + tdOff, canvasHeight - tdDist);
                    // Stop line
                    g.DrawLine(orangePen, canvasWidth / 2f + tdOff, canvasHeight - tdDist, canvasWidth / 2f + stopOff, canvasHeight - stopDist);

                    // Dots
                    g.DrawEllipse(blackPen, canvasWidth / 2f + toOff, canvasHeight - toDist, 1, 1);
                    g.DrawEllipse(blackPen, canvasWidth / 2f + tdOff, canvasHeight - tdDist, 1, 1);
                    g.DrawEllipse(blackPen, canvasWidth / 2f + stopOff, canvasHeight - stopDist, 1, 1);

                    // Labels
                    using Font drawFont = new Font("Arial", 9, FontStyle.Bold);
                    using Brush drawBrush = new SolidBrush(Color.Black);

                    g.DrawString("Takeoff", drawFont, drawBrush, canvasWidth / 2f + toOff - 50, canvasHeight - toDist - 15);
                    g.DrawString("Touchdown", drawFont, drawBrush, canvasWidth / 2f + tdOff + 5, canvasHeight - tdDist - 15);
                    g.DrawString("Stop", drawFont, drawBrush, canvasWidth / 2f + stopOff + 5, canvasHeight - stopDist - 15);
                }

                // Runway border stretched to full canvas
                Color penColor = SystemColors.Control;
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                if (Application.IsDarkModeEnabled)
                {
                    penColor = myDarkControl;
                }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                using Pen borderPen = new Pen(penColor, 5);
                g.DrawRectangle(borderPen, 0, 0, canvasWidth, canvasHeight);
            }
        }

        void pannel_DoubleClick(object sender, EventArgs e)
        {
            if (selectedFieldSize == fieldSizeFull) selectedFieldSize = fieldSizeZoom;
            else if (selectedFieldSize == fieldSizeZoom) selectedFieldSize = fieldSizeFull;
            panel.Invalidate();
        }

        private void comboBoxUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            string unit = comboBoxUnit.Text;
            var config = Config.GetInstance();
            config.Unit = unit;
            config.Save();
            controller.setUnit(unit);
        }

        private void checkBoxResult_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxResult.Checked && controller.user == "")
            {
                checkBoxResult.Checked = false;
                MessageBox.Show("Please set Username to enbale sending data.");
            }

            var config = Config.GetInstance();
            config.isSendResults = checkBoxResult.Checked;
            if (config.isSendResults && !config.hasPrivacyConfirmed)
            {
                MessageBox.Show("By enabeling, you agree that your landing result data will be temporarily stored for up to 30 days and may be shown on a public dashboard.\r\n" +
                "For more information, see the privacy policy: https://github.com/CedricPump/msfs_stol_training_tool/blob/main/doc/Privacy_Policy.md");
                config.hasPrivacyConfirmed = true;
            }
            config.Save();
        }

        private void checkBoxTelemetry_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTelemetry.Checked && controller.user == "")
            {
                checkBoxTelemetry.Checked = false;
                MessageBox.Show("Please set Username to enbale sending data.");
            }

            var config = Config.GetInstance();
            config.isSendTelemetry = checkBoxTelemetry.Checked;
            if (config.isSendTelemetry && !config.hasPrivacyConfirmed)
            {
                MessageBox.Show("By enabeling, you agree that your ingame telemetry data will be temporarily stored for up to 30 days and may be shown on a public dashboard.\r\n" +
                "For more information, see the privacy policy: https://github.com/CedricPump/msfs_stol_training_tool/blob/main/doc/Privacy_Policy.md");
                config.hasPrivacyConfirmed = true;
            }
            config.Save();
        }

        private void Timer(object sender, EventArgs e)
        {
            // just to check it periodically
            if (alwaysontop && !this.TopMost) this.TopMost = alwaysontop;

            TimeSpan elapsed = this.stopwatch.Elapsed + StopwatchOffset;
            string minus = elapsed.TotalSeconds < 0 ? "-" : " ";
            labelStopwatch.Text = string.Format("{0}{1:00}:{2:00}", minus, elapsed.Minutes, Math.Abs(elapsed.Seconds));
            if (elapsed.TotalSeconds <= 180)
            {
                if (elapsed.TotalSeconds >= 0)
                {
                    this.progressBarStopwatch.Value = (int)elapsed.TotalSeconds;
                }
                else
                {
                    this.progressBarStopwatch.Value = 0;
                }
            }

            this.textBoxAligned.Text = aligned;
            this.textBoxAligned.BackColor = alignColor;
            this.textBoxAligned.ForeColor = Color.Black;

            this.labelPlaneType.Text = "plane: " + controller.plane.GetDisplayName();

            SetDeviations();

            this.buttonPauseUnpause.Visible = controller.isPaused;
            this.buttonUnFlip.Enabled = controller.plane.isInit && controller.plane.IsFlipped() && !controller.plane.isReadonly;
            this.buttonCreatePreset.Enabled = controller.plane.isInit && controller.plane.IsStopped();
            this.buttonSetFuelStandard.Enabled = false; // controller.plane.isInit && controller.plane.IsStopped();

            // set pattern altitude
            if (controller.stol != null && controller.stol.preset != null)
            {
                int patternAlt = controller.stol.preset.getPatternAltitude();
                double currentAlt = controller.plane.Altitude;
                string arrow = "";
                if (currentAlt - 50 > patternAlt)
                {
                    arrow = "⤓";
                }
                else if (currentAlt + 50 < patternAlt)
                {
                    arrow = "⤒";
                }
                else
                {
                    arrow = "✓";
                }
                this.labelPatternAltitude.Text = $"Pattern Alt: {patternAlt} ft ASL" + arrow;
            }
        }


        public void SetDeviations()
        {
            if (controller.stol.deviations.Count == deviations) return;

            // Make sure it's a RichTextBox in the Designer or dynamically
            var box = this.richTextBoxDeviations as RichTextBox;
            if (box == null) return;

            box.Clear();

            foreach (var deviation in controller.stol.deviations)
            {
                // Choose color based on severity
                Color color = deviation.Severity switch
                {
                    0 => myDarkControlText,     // Remark
                    1 => Color.Yellow,   // Warning
                    2 => Color.Orange,   // Deviation
                    3 => Color.Red,      // Violation
                    _ => myDarkControlText
                };

                string formattedValue;
                if (Math.Abs(deviation.Value % 1) < 0.05) // roughly integer (e.g. 1.0)
                {
                    formattedValue = ((int)Math.Round(deviation.Value)).ToString();
                }
                else if (deviation.Value < 10)
                {
                    formattedValue = deviation.Value.ToString("0.0");
                }
                else
                {
                    formattedValue = deviation.Value.ToString("0");
                }

                string text = $"{deviation.Type}[{formattedValue}]";

                // Apply color and append
                box.SelectionColor = color;
                box.AppendText(text);

                // Add comma if not last
                if (deviation != controller.stol.deviations.Last())
                {
                    box.SelectionColor = myDarkControlText;
                    box.AppendText(", ");
                }
            }

            deviations = controller.stol.deviations.Count;
        }


        public void StartStopWatch()
        {
            if (!this.stopwatch.IsRunning)
            {
                this.stopwatch.Reset();
                this.StopwatchOffset = TimeSpan.Zero;
                this.stopwatch.Start();
            }
        }

        public void StopStopWatch()
        {
            this.stopwatch.Stop();
        }

        public void ResetStopWatch()
        {
            this.stopwatch.Reset();
        }

        private void buttonStartStopwatch_Click(object sender, EventArgs e)
        {
            this.StopwatchOffset = TimeSpan.FromSeconds(-stopwatchOffsetSeconds);
            this.stopwatch.Restart();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.StopwatchOffset = TimeSpan.Zero;
            this.stopwatch.Restart();
        }

        private void checkBoxOntop_CheckedChanged(object sender, EventArgs e)
        {
            this.alwaysontop = checkBoxOntop.Checked;
            this.TopMost = this.alwaysontop;
            var config = Config.GetInstance();
            config.alwaysOnTop = this.alwaysontop;
            config.Save();

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.stopwatchOffsetSeconds = (int)numericUpDownStopwatchOffest.Value;
        }

        private void labelPreset_Click(object sender, EventArgs e)
        {
            if (debug)
            {
                this.controller.reloadPresets();
            }
        }

        private void checkBoxDebugging_CheckedChanged(object sender, EventArgs e)
        {
            this.debug = checkBoxDebugging.Checked;
            Config.GetInstance().debug = this.debug;
        }

        internal void setPlanePos(GeoCoordinate initialPosition, double initialHeading, GeoCoordinate position)
        {
            this.InitailPos = initialPosition;
            this.InitialHeading = initialHeading;
            this.PlanePos = position;
            panel.Invalidate();
        }

        public void setAligned(string Test, Color color)
        {
            this.aligned = Test;
            this.alignColor = color;
        }

        public void setWind(double winddir, double wind)
        {
            this.WindDir = winddir;
            this.Wind = wind;
            panelWind.Invalidate();
        }

        public void setCollisionWheels()
        {
            panelCollisions.Invalidate();
        }

        private void panelWind_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = SystemColors.Control;
            Color foreColor = SystemColors.ControlText;
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (Application.IsDarkModeEnabled)
            {
                backColor = myDarkControl;
                foreColor = Color.White;
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CA1416 // Validate platform compatibility

            labelWind.Text = $"{this.Wind,4:0.0}";

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(backColor);

            AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 10); // width, height
            Pen arrowPen = new Pen(foreColor, 2)
            {
                CustomEndCap = bigArrow
            };

            // Draw wind compass circle
            g.DrawEllipse(arrowPen, 2, 2, 60, 60);

            // Convert to canvas angle (0° = up, rotate clockwise)
            double windDirTo = (WindDir + 180) % 360;
            double angleRad = (windDirTo - 90) * Math.PI / 180.0;

            float centerX = 32;
            float centerY = 32;
            float length = 25;

            // Calculate start and end points so arrow pivots around center
            float dx = (float)(Math.Cos(angleRad) * length);
            float dy = (float)(Math.Sin(angleRad) * length);

            PointF start = new PointF(centerX + dx, centerY - dy); // tail
            PointF end = new PointF(centerX - dx, centerY + dy);   // arrowhead

            g.DrawLine(arrowPen, start, end);
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void textBoxSessionKey_TextChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownTransparency_ValueChanged(object sender, EventArgs e)
        {
            this.Opacity = (double)(1 - numericUpDownTransparency.Value / 100);
        }

        private void checkBoxSaveRecording_CheckedChanged(object sender, EventArgs e)
        {
            var config = Config.GetInstance();
            config.enableGPXRecodering = checkBoxSaveRecording.Checked;
            config.Save();
        }

        private void linkLabelRecordings_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string exportPath = Config.GetInstance().RecordingExportPath;

            if (Directory.Exists(exportPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", exportPath);
            }
            else
            {
                MessageBox.Show("Export directory does not exist: " + exportPath);
            }
        }

        private void buttonUnFlip_Click(object sender, EventArgs e)
        {
            this.controller.unflip();
        }

        private void panelCollisions_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = SystemColors.Control;
            Color foreColor = SystemColors.ControlText;
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (Application.IsDarkModeEnabled)
            {
                backColor = myDarkControl;
                foreColor = Color.White;
            }
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning restore CA1416 // Validate platform compatibility

            Graphics g = e.Graphics;
            g.Clear(Color.Transparent); // Background green
            g.DrawImage(panelCollisions.BackgroundImage, new Rectangle(0, 0, 68, 68));
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // draw Left Wheel Colision
            Rectangle rectWL = new Rectangle(24, 19, 4, 6);

            Pen pen = new Pen(Color.Transparent, 2);
            SolidBrush brush = new SolidBrush(Color.Transparent);
            if (this.controller.plane.LeftGearOnGround())
            {
                pen = new Pen(Color.Green, 2);
                brush = new SolidBrush(Color.LightGreen);
            }
            else
            {
                pen = new Pen(Color.Gray, 2);
                brush = new SolidBrush(Color.LightGray);
            }
            g.DrawEllipse(pen, rectWL);
            g.FillEllipse(brush, rectWL);

            // draw Right Wheel Colision
            Rectangle rectWR = new Rectangle(39, 19, 4, 6);
            if (this.controller.plane.RightGearOnGround())
            {
                pen = new Pen(Color.Green, 2);
                brush = new SolidBrush(Color.LightGreen);
            }
            else
            {
                pen = new Pen(Color.Gray, 2);
                brush = new SolidBrush(Color.LightGray);
            }
            g.DrawEllipse(pen, rectWR);
            g.FillEllipse(brush, rectWR);

            // draw Tail Wheel Colision
            Rectangle rectWT = new Rectangle(32, 49, 3, 4);
            if (this.controller.plane.TailNoseGearOnGround())
            {
                pen = new Pen(Color.Green, 2);
                brush = new SolidBrush(Color.LightGreen);
            }
            else
            {
                pen = new Pen(Color.Gray, 2);
                brush = new SolidBrush(Color.LightGray);
            }
            g.DrawEllipse(pen, rectWT);
            g.FillEllipse(brush, rectWT);

            // draw Left Wing Tip
            Rectangle rectWingL = new Rectangle(1, 23, 3, 10);
            if (this.controller.plane.WingtipOnGroundL())
            {
                pen = new Pen(Color.Red, 2);
                brush = new SolidBrush(Color.Pink);
            }
            else
            {
                pen = new Pen(Color.Gray, 1);
                brush = new SolidBrush(Color.Transparent);
            }
            g.DrawEllipse(pen, rectWingL);
            g.FillEllipse(brush, rectWingL);

            // draw Right Wing Tip
            Rectangle rectWingR = new Rectangle(63, 23, 3, 10);
            if (this.controller.plane.WingtipOnGroundR())
            {
                pen = new Pen(Color.Red, 2);
                brush = new SolidBrush(Color.Pink);
            }
            else
            {
                pen = new Pen(Color.Gray, 1);
                brush = new SolidBrush(Color.Transparent);
            }
            g.DrawEllipse(pen, rectWingR);
            g.FillEllipse(brush, rectWingR);

            // draw Prop
            Rectangle rectProp = new Rectangle(26, 12, 15, 2);
            if (this.controller.plane.IsPropstrike())
            {
                pen = new Pen(Color.Red, 2);
                brush = new SolidBrush(Color.Pink);
            }
            else if (this.controller.plane.IsEngineOn)
            {
                pen = new Pen(Color.Green, 2);
                brush = new SolidBrush(Color.LightGreen);
            }
            else
            {
                pen = new Pen(Color.Gray, 1);
                brush = new SolidBrush(Color.Transparent);
            }
            g.DrawEllipse(pen, rectProp);
            g.FillEllipse(brush, rectProp);
        }

        private void buttonAutoSelect_Click(object sender, EventArgs e)
        {
            this.controller.AutoSetPreset();
        }

        private void comboBoxPreset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        internal void setSelctedPreset(string title)
        {
            comboBoxPreset.Text = title;
        }

        private void buttonClearResultBox_Click(object sender, EventArgs e)
        {
            clearResultBox();
        }

        private void buttonPauseUnpause_Click(object sender, EventArgs e)
        {
            controller.Unpause();
        }

        private async void buttonCheckUpdate_Click(object sender, EventArgs e)
        {
            var alwaysontop = this.alwaysontop;
            this.TopMost = false;
            this.TopLevel = false;
            await controller.CheckForUpdateManual();
            this.TopLevel = true;
            this.TopMost = alwaysontop;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void FormUI_Load(object sender, EventArgs e)
        {

        }

        private void buttonSetFuelStandard_Click(object sender, EventArgs e)
        {
            controller.setFuelStandard();
        }

        private void richTextBoxDeviations_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
