namespace STOL_Training_Tool_Core.UI
{
    partial class FormUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUI));
            textBoxResult = new System.Windows.Forms.TextBox();
            textBoxUser = new System.Windows.Forms.TextBox();
            labelPreset = new System.Windows.Forms.Label();
            comboBoxPreset = new System.Windows.Forms.ComboBox();
            buttonApplyPreset = new System.Windows.Forms.Button();
            buttonTeleport = new System.Windows.Forms.Button();
            buttonSetRefPos = new System.Windows.Forms.Button();
            textBoxStatus = new System.Windows.Forms.TextBox();
            buttonCreatePreset = new System.Windows.Forms.Button();
            panel = new System.Windows.Forms.Panel();
            comboBoxUnit = new System.Windows.Forms.ComboBox();
            checkBoxResult = new System.Windows.Forms.CheckBox();
            checkBoxTelemetry = new System.Windows.Forms.CheckBox();
            timer1 = new System.Windows.Forms.Timer(components);
            labelStopwatch = new System.Windows.Forms.Label();
            progressBarStopwatch = new System.Windows.Forms.ProgressBar();
            buttonStartStopwatch = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            checkBoxOntop = new System.Windows.Forms.CheckBox();
            numericUpDownStopwatchOffest = new System.Windows.Forms.NumericUpDown();
            label2 = new System.Windows.Forms.Label();
            checkBoxDebugging = new System.Windows.Forms.CheckBox();
            panelWind = new System.Windows.Forms.Panel();
            labelWind = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            textBoxAligned = new System.Windows.Forms.TextBox();
            labelName = new System.Windows.Forms.Label();
            labelSession = new System.Windows.Forms.Label();
            textBoxSessionKey = new System.Windows.Forms.TextBox();
            numericUpDownTransparency = new System.Windows.Forms.NumericUpDown();
            checkBoxSaveRecording = new System.Windows.Forms.CheckBox();
            linkLabelRecordings = new System.Windows.Forms.LinkLabel();
            buttonUnFlip = new System.Windows.Forms.Button();
            panelCollisions = new System.Windows.Forms.Panel();
            checkBoxPropStrike = new System.Windows.Forms.CheckBox();
            buttonAutoSelect = new System.Windows.Forms.Button();
            labelPlaneType = new System.Windows.Forms.Label();
            richTextBoxDeviations = new System.Windows.Forms.RichTextBox();
            label4 = new System.Windows.Forms.Label();
            buttonClearResultBox = new System.Windows.Forms.Button();
            buttonPauseUnpause = new System.Windows.Forms.Button();
            buttonCheckUpdate = new System.Windows.Forms.Button();
            label5 = new System.Windows.Forms.Label();
            toolTiphint = new System.Windows.Forms.ToolTip(components);
            buttonSetFuelStandard = new System.Windows.Forms.Button();
            toolTipZoom = new System.Windows.Forms.ToolTip(components);
            labelPatternAltitude = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDownStopwatchOffest).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTransparency).BeginInit();
            SuspendLayout();
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            textBoxResult.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBoxResult.Location = new System.Drawing.Point(12, 58);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ReadOnly = true;
            textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBoxResult.Size = new System.Drawing.Size(378, 579);
            textBoxResult.TabIndex = 1;
            // 
            // textBoxUser
            // 
            textBoxUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            textBoxUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxUser.Location = new System.Drawing.Point(710, 43);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.Size = new System.Drawing.Size(209, 23);
            textBoxUser.TabIndex = 2;
            toolTiphint.SetToolTip(textBoxUser, "You can upload your training data to a database. Leave the input empty to skip uploading.");
            textBoxUser.TextChanged += textBoxUser_TextChanged;
            textBoxUser.KeyDown += textBoxUser_KeyDown;
            // 
            // labelPreset
            // 
            labelPreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelPreset.AutoSize = true;
            labelPreset.Location = new System.Drawing.Point(709, 192);
            labelPreset.Name = "labelPreset";
            labelPreset.Size = new System.Drawing.Size(39, 15);
            labelPreset.TabIndex = 3;
            labelPreset.Text = "Preset";
            labelPreset.Click += labelPreset_Click;
            // 
            // comboBoxPreset
            // 
            comboBoxPreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            comboBoxPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxPreset.FormattingEnabled = true;
            comboBoxPreset.Location = new System.Drawing.Point(710, 210);
            comboBoxPreset.Name = "comboBoxPreset";
            comboBoxPreset.Size = new System.Drawing.Size(211, 23);
            comboBoxPreset.TabIndex = 0;
            comboBoxPreset.SelectedIndexChanged += comboBoxPreset_SelectedIndexChanged;
            // 
            // buttonApplyPreset
            // 
            buttonApplyPreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonApplyPreset.AutoSize = true;
            buttonApplyPreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonApplyPreset.Location = new System.Drawing.Point(709, 272);
            buttonApplyPreset.Name = "buttonApplyPreset";
            buttonApplyPreset.Size = new System.Drawing.Size(211, 27);
            buttonApplyPreset.TabIndex = 5;
            buttonApplyPreset.Text = "Apply";
            toolTiphint.SetToolTip(buttonApplyPreset, "apply selected preset");
            buttonApplyPreset.Click += buttonApplyPreset_Click;
            // 
            // buttonTeleport
            // 
            buttonTeleport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonTeleport.AutoSize = true;
            buttonTeleport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonTeleport.Location = new System.Drawing.Point(709, 338);
            buttonTeleport.Name = "buttonTeleport";
            buttonTeleport.Size = new System.Drawing.Size(211, 27);
            buttonTeleport.TabIndex = 6;
            buttonTeleport.Text = "Teleport";
            toolTiphint.SetToolTip(buttonTeleport, "teleport to reference line");
            buttonTeleport.Click += buttonTeleport_Click;
            // 
            // buttonSetRefPos
            // 
            buttonSetRefPos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonSetRefPos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonSetRefPos.Location = new System.Drawing.Point(710, 305);
            buttonSetRefPos.Name = "buttonSetRefPos";
            buttonSetRefPos.Size = new System.Drawing.Size(211, 27);
            buttonSetRefPos.TabIndex = 7;
            buttonSetRefPos.Text = "Set Start";
            toolTiphint.SetToolTip(buttonSetRefPos, "set reference point to current location");
            buttonSetRefPos.Click += buttonSetRefPos_Click;
            // 
            // textBoxStatus
            // 
            textBoxStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxStatus.Location = new System.Drawing.Point(12, 714);
            textBoxStatus.Name = "textBoxStatus";
            textBoxStatus.ReadOnly = true;
            textBoxStatus.Size = new System.Drawing.Size(691, 23);
            textBoxStatus.TabIndex = 8;
            textBoxStatus.Text = "Status";
            textBoxStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonCreatePreset
            // 
            buttonCreatePreset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonCreatePreset.AutoSize = true;
            buttonCreatePreset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonCreatePreset.Location = new System.Drawing.Point(709, 404);
            buttonCreatePreset.Name = "buttonCreatePreset";
            buttonCreatePreset.Size = new System.Drawing.Size(211, 27);
            buttonCreatePreset.TabIndex = 9;
            buttonCreatePreset.Text = "Create Preset";
            buttonCreatePreset.Click += buttonCreatePreset_Click;
            // 
            // panel
            // 
            panel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            panel.BackgroundImage = (System.Drawing.Image)resources.GetObject("panel.BackgroundImage");
            panel.Location = new System.Drawing.Point(396, 29);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(307, 679);
            panel.TabIndex = 10;
            toolTipZoom.SetToolTip(panel, "double click to zoom");
            panel.Paint += panel_Paint;
            panel.DoubleClick += pannel_DoubleClick;
            panel.Resize += panel_Resize;
            // 
            // comboBoxUnit
            // 
            comboBoxUnit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            comboBoxUnit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            comboBoxUnit.FormattingEnabled = true;
            comboBoxUnit.Location = new System.Drawing.Point(710, 166);
            comboBoxUnit.Name = "comboBoxUnit";
            comboBoxUnit.Size = new System.Drawing.Size(211, 23);
            comboBoxUnit.TabIndex = 11;
            comboBoxUnit.SelectedIndexChanged += comboBoxUnit_SelectedIndexChanged;
            // 
            // checkBoxResult
            // 
            checkBoxResult.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxResult.AutoSize = true;
            checkBoxResult.Location = new System.Drawing.Point(713, 116);
            checkBoxResult.Name = "checkBoxResult";
            checkBoxResult.Size = new System.Drawing.Size(88, 19);
            checkBoxResult.TabIndex = 12;
            checkBoxResult.Text = "send results";
            toolTiphint.SetToolTip(checkBoxResult, "sends STOL results\\r\\nBy enabeling, you agree that your landing result data will be temporarily stored for up to 30 days and may be shown on a public dashboard.");
            checkBoxResult.CheckedChanged += checkBoxResult_CheckedChanged;
            // 
            // checkBoxTelemetry
            // 
            checkBoxTelemetry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxTelemetry.AutoSize = true;
            checkBoxTelemetry.Location = new System.Drawing.Point(712, 141);
            checkBoxTelemetry.Name = "checkBoxTelemetry";
            checkBoxTelemetry.Size = new System.Drawing.Size(104, 19);
            checkBoxTelemetry.TabIndex = 13;
            checkBoxTelemetry.Text = "send telemetry";
            toolTiphint.SetToolTip(checkBoxTelemetry, "sends Sim flight telemetry\\r\\nBy enabeling, you agree that your landing result data will be temporarily stored for up to 30 days and may be shown on a public dashboard.");
            checkBoxTelemetry.CheckedChanged += checkBoxTelemetry_CheckedChanged;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += Timer;
            // 
            // labelStopwatch
            // 
            labelStopwatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            labelStopwatch.AutoSize = true;
            labelStopwatch.Font = new System.Drawing.Font("Consolas", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelStopwatch.Location = new System.Drawing.Point(711, 659);
            labelStopwatch.Name = "labelStopwatch";
            labelStopwatch.Size = new System.Drawing.Size(132, 41);
            labelStopwatch.TabIndex = 14;
            labelStopwatch.Text = " 00:00";
            labelStopwatch.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressBarStopwatch
            // 
            progressBarStopwatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            progressBarStopwatch.Location = new System.Drawing.Point(709, 714);
            progressBarStopwatch.Name = "progressBarStopwatch";
            progressBarStopwatch.Size = new System.Drawing.Size(210, 23);
            progressBarStopwatch.TabIndex = 15;
            // 
            // buttonStartStopwatch
            // 
            buttonStartStopwatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonStartStopwatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonStartStopwatch.Location = new System.Drawing.Point(843, 684);
            buttonStartStopwatch.Name = "buttonStartStopwatch";
            buttonStartStopwatch.Size = new System.Drawing.Size(76, 24);
            buttonStartStopwatch.TabIndex = 16;
            buttonStartStopwatch.Text = "T-Offset";
            toolTiphint.SetToolTip(buttonStartStopwatch, "Start timer with negative offset countdown");
            buttonStartStopwatch.Click += buttonStartStopwatch_Click;
            // 
            // button1
            // 
            button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button1.Location = new System.Drawing.Point(844, 655);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 18;
            button1.Text = "Start";
            toolTiphint.SetToolTip(button1, "start timer manually, continues through takeoff");
            button1.Click += buttonStart_Click;
            // 
            // checkBoxOntop
            // 
            checkBoxOntop.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            checkBoxOntop.AutoSize = true;
            checkBoxOntop.Location = new System.Drawing.Point(820, 5);
            checkBoxOntop.Name = "checkBoxOntop";
            checkBoxOntop.Size = new System.Drawing.Size(99, 19);
            checkBoxOntop.TabIndex = 19;
            checkBoxOntop.Text = "always on top";
            checkBoxOntop.CheckedChanged += checkBoxOntop_CheckedChanged;
            // 
            // numericUpDownStopwatchOffest
            // 
            numericUpDownStopwatchOffest.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownStopwatchOffest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            numericUpDownStopwatchOffest.Location = new System.Drawing.Point(711, 626);
            numericUpDownStopwatchOffest.Name = "numericUpDownStopwatchOffest";
            numericUpDownStopwatchOffest.Size = new System.Drawing.Size(208, 23);
            numericUpDownStopwatchOffest.TabIndex = 20;
            numericUpDownStopwatchOffest.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numericUpDownStopwatchOffest.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(710, 608);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(39, 15);
            label2.TabIndex = 21;
            label2.Text = "Offset";
            toolTiphint.SetToolTip(label2, "Always listen to airboss for timing instructions");
            // 
            // checkBoxDebugging
            // 
            checkBoxDebugging.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            checkBoxDebugging.AutoSize = true;
            checkBoxDebugging.Location = new System.Drawing.Point(754, 5);
            checkBoxDebugging.Name = "checkBoxDebugging";
            checkBoxDebugging.Size = new System.Drawing.Size(60, 19);
            checkBoxDebugging.TabIndex = 22;
            checkBoxDebugging.Text = "debug";
            toolTiphint.SetToolTip(checkBoxDebugging, "enable debug output. do NOT use in copetition");
            checkBoxDebugging.CheckedChanged += checkBoxDebugging_CheckedChanged;
            // 
            // panelWind
            // 
            panelWind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            panelWind.Location = new System.Drawing.Point(709, 491);
            panelWind.Name = "panelWind";
            panelWind.Size = new System.Drawing.Size(69, 68);
            panelWind.TabIndex = 23;
            panelWind.Paint += panelWind_Paint;
            // 
            // labelWind
            // 
            labelWind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelWind.AutoSize = true;
            labelWind.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelWind.Location = new System.Drawing.Point(784, 506);
            labelWind.Name = "labelWind";
            labelWind.Size = new System.Drawing.Size(60, 25);
            labelWind.TabIndex = 24;
            labelWind.Text = "--,- ft";
            labelWind.Click += label3_Click;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(784, 491);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(35, 15);
            label3.TabIndex = 25;
            label3.Text = "Wind";
            // 
            // textBoxAligned
            // 
            textBoxAligned.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxAligned.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxAligned.Location = new System.Drawing.Point(12, 29);
            textBoxAligned.Name = "textBoxAligned";
            textBoxAligned.ReadOnly = true;
            textBoxAligned.Size = new System.Drawing.Size(378, 23);
            textBoxAligned.TabIndex = 26;
            textBoxAligned.Text = "...";
            textBoxAligned.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            textBoxAligned.TextChanged += textBox1_TextChanged;
            // 
            // labelName
            // 
            labelName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelName.AutoSize = true;
            labelName.Font = new System.Drawing.Font("Segoe UI", 6F);
            labelName.Location = new System.Drawing.Point(710, 29);
            labelName.Name = "labelName";
            labelName.Size = new System.Drawing.Size(26, 11);
            labelName.TabIndex = 27;
            labelName.Text = "Name";
            labelName.Click += label4_Click;
            // 
            // labelSession
            // 
            labelSession.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelSession.AutoSize = true;
            labelSession.Font = new System.Drawing.Font("Segoe UI", 6F);
            labelSession.Location = new System.Drawing.Point(707, 69);
            labelSession.Name = "labelSession";
            labelSession.Size = new System.Drawing.Size(31, 11);
            labelSession.TabIndex = 28;
            labelSession.Text = "Session";
            labelSession.Click += label4_Click_1;
            // 
            // textBoxSessionKey
            // 
            textBoxSessionKey.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            textBoxSessionKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxSessionKey.Location = new System.Drawing.Point(710, 83);
            textBoxSessionKey.Name = "textBoxSessionKey";
            textBoxSessionKey.Size = new System.Drawing.Size(209, 23);
            textBoxSessionKey.TabIndex = 29;
            toolTiphint.SetToolTip(textBoxSessionKey, "additional session key - use if instructed");
            textBoxSessionKey.KeyDown += textBoxSessionKey_KeyDown;
            // 
            // numericUpDownTransparency
            // 
            numericUpDownTransparency.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            numericUpDownTransparency.BorderStyle = System.Windows.Forms.BorderStyle.None;
            numericUpDownTransparency.Font = new System.Drawing.Font("Segoe UI", 8F);
            numericUpDownTransparency.Increment = new decimal(new int[] { 10, 0, 0, 0 });
            numericUpDownTransparency.Location = new System.Drawing.Point(712, 6);
            numericUpDownTransparency.Maximum = new decimal(new int[] { 80, 0, 0, 0 });
            numericUpDownTransparency.Name = "numericUpDownTransparency";
            numericUpDownTransparency.Size = new System.Drawing.Size(35, 18);
            numericUpDownTransparency.TabIndex = 30;
            numericUpDownTransparency.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            numericUpDownTransparency.ValueChanged += numericUpDownTransparency_ValueChanged;
            // 
            // checkBoxSaveRecording
            // 
            checkBoxSaveRecording.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxSaveRecording.AutoSize = true;
            checkBoxSaveRecording.Checked = true;
            checkBoxSaveRecording.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxSaveRecording.Location = new System.Drawing.Point(836, 116);
            checkBoxSaveRecording.Name = "checkBoxSaveRecording";
            checkBoxSaveRecording.Size = new System.Drawing.Size(85, 19);
            checkBoxSaveRecording.TabIndex = 100;
            checkBoxSaveRecording.Text = "record GPX";
            toolTiphint.SetToolTip(checkBoxSaveRecording, "Record your flight path as GPX or CSV file for later review or sharing");
            checkBoxSaveRecording.UseVisualStyleBackColor = true;
            checkBoxSaveRecording.CheckedChanged += checkBoxSaveRecording_CheckedChanged;
            // 
            // linkLabelRecordings
            // 
            linkLabelRecordings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            linkLabelRecordings.AutoSize = true;
            linkLabelRecordings.Location = new System.Drawing.Point(820, 142);
            linkLabelRecordings.Name = "linkLabelRecordings";
            linkLabelRecordings.Size = new System.Drawing.Size(99, 15);
            linkLabelRecordings.TabIndex = 101;
            linkLabelRecordings.TabStop = true;
            linkLabelRecordings.Text = "GPX export folder";
            linkLabelRecordings.LinkClicked += linkLabelRecordings_LinkClicked;
            // 
            // buttonUnFlip
            // 
            buttonUnFlip.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonUnFlip.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonUnFlip.Location = new System.Drawing.Point(709, 371);
            buttonUnFlip.Name = "buttonUnFlip";
            buttonUnFlip.Size = new System.Drawing.Size(211, 27);
            buttonUnFlip.TabIndex = 102;
            buttonUnFlip.Text = "UnFlip";
            toolTiphint.SetToolTip(buttonUnFlip, "unflip plane");
            buttonUnFlip.UseVisualStyleBackColor = true;
            buttonUnFlip.Click += buttonUnFlip_Click;
            // 
            // panelCollisions
            // 
            panelCollisions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            panelCollisions.BackgroundImage = (System.Drawing.Image)resources.GetObject("panelCollisions.BackgroundImage");
            panelCollisions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            panelCollisions.Location = new System.Drawing.Point(852, 491);
            panelCollisions.Name = "panelCollisions";
            panelCollisions.Size = new System.Drawing.Size(68, 68);
            panelCollisions.TabIndex = 103;
            panelCollisions.Paint += panelCollisions_Paint;
            // 
            // checkBoxPropStrike
            // 
            checkBoxPropStrike.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            checkBoxPropStrike.AutoSize = true;
            checkBoxPropStrike.Checked = true;
            checkBoxPropStrike.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBoxPropStrike.Enabled = false;
            checkBoxPropStrike.Location = new System.Drawing.Point(373, 5);
            checkBoxPropStrike.Name = "checkBoxPropStrike";
            checkBoxPropStrike.Size = new System.Drawing.Size(120, 19);
            checkBoxPropStrike.TabIndex = 104;
            checkBoxPropStrike.Text = "enable prop strike";
            checkBoxPropStrike.UseVisualStyleBackColor = true;
            // 
            // buttonAutoSelect
            // 
            buttonAutoSelect.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonAutoSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonAutoSelect.Location = new System.Drawing.Point(710, 239);
            buttonAutoSelect.Name = "buttonAutoSelect";
            buttonAutoSelect.Size = new System.Drawing.Size(212, 27);
            buttonAutoSelect.TabIndex = 105;
            buttonAutoSelect.Text = "Auto Select";
            toolTiphint.SetToolTip(buttonAutoSelect, "Automatically selects the NEAREST reference line");
            buttonAutoSelect.UseVisualStyleBackColor = true;
            buttonAutoSelect.Click += buttonAutoSelect_Click;
            // 
            // labelPlaneType
            // 
            labelPlaneType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            labelPlaneType.AutoSize = true;
            labelPlaneType.Location = new System.Drawing.Point(12, 6);
            labelPlaneType.Name = "labelPlaneType";
            labelPlaneType.Size = new System.Drawing.Size(58, 15);
            labelPlaneType.TabIndex = 107;
            labelPlaneType.Text = "Unknown";
            labelPlaneType.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // richTextBoxDeviations
            // 
            richTextBoxDeviations.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            richTextBoxDeviations.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBoxDeviations.Location = new System.Drawing.Point(12, 658);
            richTextBoxDeviations.Name = "richTextBoxDeviations";
            richTextBoxDeviations.ReadOnly = true;
            richTextBoxDeviations.Size = new System.Drawing.Size(378, 50);
            richTextBoxDeviations.TabIndex = 108;
            richTextBoxDeviations.Text = "";
            richTextBoxDeviations.TextChanged += richTextBoxDeviations_TextChanged;
            // 
            // label4
            // 
            label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 640);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(129, 15);
            label4.TabIndex = 109;
            label4.Text = "remarks and deviations";
            // 
            // buttonClearResultBox
            // 
            buttonClearResultBox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonClearResultBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            buttonClearResultBox.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            buttonClearResultBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonClearResultBox.Location = new System.Drawing.Point(367, 636);
            buttonClearResultBox.Name = "buttonClearResultBox";
            buttonClearResultBox.Size = new System.Drawing.Size(23, 23);
            buttonClearResultBox.TabIndex = 110;
            buttonClearResultBox.Text = "🗑";
            toolTiphint.SetToolTip(buttonClearResultBox, "clear result log");
            buttonClearResultBox.UseVisualStyleBackColor = true;
            buttonClearResultBox.Click += buttonClearResultBox_Click;
            // 
            // buttonPauseUnpause
            // 
            buttonPauseUnpause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonPauseUnpause.Font = new System.Drawing.Font("Bahnschrift Condensed", 9F);
            buttonPauseUnpause.Location = new System.Drawing.Point(499, 3);
            buttonPauseUnpause.Name = "buttonPauseUnpause";
            buttonPauseUnpause.Size = new System.Drawing.Size(23, 23);
            buttonPauseUnpause.TabIndex = 111;
            buttonPauseUnpause.Text = "▶";
            buttonPauseUnpause.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            toolTiphint.SetToolTip(buttonPauseUnpause, "Unpause");
            buttonPauseUnpause.UseVisualStyleBackColor = true;
            buttonPauseUnpause.Visible = false;
            buttonPauseUnpause.Click += buttonPauseUnpause_Click;
            // 
            // buttonCheckUpdate
            // 
            buttonCheckUpdate.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            buttonCheckUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonCheckUpdate.ForeColor = System.Drawing.SystemColors.ControlText;
            buttonCheckUpdate.Location = new System.Drawing.Point(528, 2);
            buttonCheckUpdate.Name = "buttonCheckUpdate";
            buttonCheckUpdate.Size = new System.Drawing.Size(95, 23);
            buttonCheckUpdate.TabIndex = 112;
            buttonCheckUpdate.Text = "check update";
            buttonCheckUpdate.UseVisualStyleBackColor = true;
            buttonCheckUpdate.Click += buttonCheckUpdate_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(628, 6);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(75, 15);
            label5.TabIndex = 113;
            label5.Text = "transparency";
            label5.Click += label5_Click;
            // 
            // toolTiphint
            // 
            toolTiphint.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            toolTiphint.ToolTipTitle = "Hint";
            // 
            // buttonSetFuelStandard
            // 
            buttonSetFuelStandard.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            buttonSetFuelStandard.AutoSize = true;
            buttonSetFuelStandard.Enabled = false;
            buttonSetFuelStandard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonSetFuelStandard.Location = new System.Drawing.Point(709, 439);
            buttonSetFuelStandard.Name = "buttonSetFuelStandard";
            buttonSetFuelStandard.Size = new System.Drawing.Size(211, 27);
            buttonSetFuelStandard.TabIndex = 115;
            buttonSetFuelStandard.Text = "Set Default Fuel";
            toolTiphint.SetToolTip(buttonSetFuelStandard, "Disabled: API restrictions");
            buttonSetFuelStandard.Click += buttonSetFuelStandard_Click;
            // 
            // toolTipZoom
            // 
            toolTipZoom.AutomaticDelay = 0;
            toolTipZoom.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            toolTipZoom.ToolTipTitle = "Zoom";
            // 
            // labelPatternAltitude
            // 
            labelPatternAltitude.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            labelPatternAltitude.AutoSize = true;
            labelPatternAltitude.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            labelPatternAltitude.Location = new System.Drawing.Point(707, 562);
            labelPatternAltitude.Name = "labelPatternAltitude";
            labelPatternAltitude.Size = new System.Drawing.Size(171, 25);
            labelPatternAltitude.TabIndex = 114;
            labelPatternAltitude.Text = "Pattern Alt: ---- ft _";
            // 
            // FormUI
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(931, 748);
            Controls.Add(buttonSetFuelStandard);
            Controls.Add(labelPatternAltitude);
            Controls.Add(label5);
            Controls.Add(buttonCheckUpdate);
            Controls.Add(buttonPauseUnpause);
            Controls.Add(buttonClearResultBox);
            Controls.Add(label4);
            Controls.Add(richTextBoxDeviations);
            Controls.Add(labelPlaneType);
            Controls.Add(buttonAutoSelect);
            Controls.Add(checkBoxPropStrike);
            Controls.Add(panelCollisions);
            Controls.Add(buttonUnFlip);
            Controls.Add(linkLabelRecordings);
            Controls.Add(checkBoxSaveRecording);
            Controls.Add(numericUpDownTransparency);
            Controls.Add(textBoxSessionKey);
            Controls.Add(labelSession);
            Controls.Add(labelName);
            Controls.Add(textBoxAligned);
            Controls.Add(label3);
            Controls.Add(labelWind);
            Controls.Add(panelWind);
            Controls.Add(checkBoxDebugging);
            Controls.Add(label2);
            Controls.Add(numericUpDownStopwatchOffest);
            Controls.Add(checkBoxOntop);
            Controls.Add(button1);
            Controls.Add(buttonStartStopwatch);
            Controls.Add(progressBarStopwatch);
            Controls.Add(labelStopwatch);
            Controls.Add(checkBoxTelemetry);
            Controls.Add(checkBoxResult);
            Controls.Add(comboBoxUnit);
            Controls.Add(panel);
            Controls.Add(buttonCreatePreset);
            Controls.Add(textBoxStatus);
            Controls.Add(buttonSetRefPos);
            Controls.Add(buttonTeleport);
            Controls.Add(buttonApplyPreset);
            Controls.Add(comboBoxPreset);
            Controls.Add(labelPreset);
            Controls.Add(textBoxUser);
            Controls.Add(textBoxResult);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormUI";
            Text = "STOL Training Tool";
            Load += FormUI_Load;
            ((System.ComponentModel.ISupportInitialize)numericUpDownStopwatchOffest).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownTransparency).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox textBoxResult;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label labelPreset;
        private System.Windows.Forms.ComboBox comboBoxPreset;
        private System.Windows.Forms.Button buttonApplyPreset;
        private System.Windows.Forms.Button buttonTeleport;
        private System.Windows.Forms.Button buttonSetRefPos;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Button buttonCreatePreset;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ComboBox comboBoxUnit;
        private System.Windows.Forms.CheckBox checkBoxResult;
        private System.Windows.Forms.CheckBox checkBoxTelemetry;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelStopwatch;
        private System.Windows.Forms.ProgressBar progressBarStopwatch;
        private System.Windows.Forms.Button buttonStartStopwatch;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxOntop;
        private System.Windows.Forms.NumericUpDown numericUpDownStopwatchOffest;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxDebugging;
        private System.Windows.Forms.Panel panelWind;
        private System.Windows.Forms.Label labelWind;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxAligned;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelSession;
        private System.Windows.Forms.TextBox textBoxSessionKey;
        private System.Windows.Forms.NumericUpDown numericUpDownTransparency;
        private System.Windows.Forms.CheckBox checkBoxSaveRecording;
        private System.Windows.Forms.LinkLabel linkLabelRecordings;
        private System.Windows.Forms.Button buttonUnFlip;
        private System.Windows.Forms.Panel panelCollisions;
        private System.Windows.Forms.CheckBox checkBoxPropStrike;
        private System.Windows.Forms.Button buttonAutoSelect;
        private System.Windows.Forms.Label labelPlaneType;
        private System.Windows.Forms.RichTextBox richTextBoxDeviations;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonClearResultBox;
        private System.Windows.Forms.Button buttonPauseUnpause;
        private System.Windows.Forms.Button buttonCheckUpdate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTiphint;
        private System.Windows.Forms.ToolTip toolTipGPX;
        private System.Windows.Forms.ToolTip toolTipTimer;
        private System.Windows.Forms.ToolTip toolTipTimerOffset;
        private System.Windows.Forms.ToolTip toolTipZoom;
        private System.Windows.Forms.Label labelPatternAltitude;
        private System.Windows.Forms.Button buttonSetFuelStandard;
    }
}