using System;
using System.Drawing;
using System.Windows.Forms;

namespace STOL_Training_Tool_Core.UI
{
    /// <summary>
    /// Install/update confirmation for the MSFS toolbar panel. Sized to fit
    /// the full Community path (a TaskDialog can't be widened).
    /// Returns DialogResult.Yes = Install, DialogResult.Retry = Change
    /// Location..., DialogResult.Cancel = cancel.
    /// </summary>
    public class FormInstallPanel : Form
    {
        public FormInstallPanel(string communityPath)
        {
            Text = "Install MSFS Panel";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoScaleMode = AutoScaleMode.Font;
            Font = new Font("Segoe UI", 9F);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(12);

            var layout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Dock = DockStyle.Fill
            };

            var heading = new Label
            {
                Text = "Install/update the STOL toolbar panel to:",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6)
            };

            // Read-only textbox so a long path is fully visible and copyable.
            int maxWidth = (int)(Screen.FromControl(this).WorkingArea.Width * 0.8);
            int textWidth = TextRenderer.MeasureText(communityPath, Font).Width + 16;
            var pathBox = new TextBox
            {
                Text = communityPath,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Width = Math.Min(Math.Max(textWidth, 420), maxWidth),
                Margin = new Padding(0, 0, 0, 12),
                TabStop = false
            };

            var buttons = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                WrapContents = false
            };
            // Flat like the rest of the app - Standard-style buttons draw a
            // light face under white text in dark mode.
            var cancelButton = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, AutoSize = true, FlatStyle = FlatStyle.Flat };
            var changeButton = new Button { Text = "Change Location...", DialogResult = DialogResult.Retry, AutoSize = true, FlatStyle = FlatStyle.Flat };
            var installButton = new Button { Text = "Install", DialogResult = DialogResult.Yes, AutoSize = true, FlatStyle = FlatStyle.Flat };
            buttons.Controls.AddRange(new Control[] { cancelButton, changeButton, installButton });

            layout.Controls.Add(heading);
            layout.Controls.Add(pathBox);
            layout.Controls.Add(buttons);
            Controls.Add(layout);

            AcceptButton = installButton;
            CancelButton = cancelButton;
            Shown += (s, e) => installButton.Focus();
        }
    }
}
