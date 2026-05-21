using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using STOL_Training_Tool_Core.Core;
using STOL_Training_Tool_Core.Model;
using Newtonsoft.Json;

namespace STOL_Training_Tool_Core.UI
{
    public partial class FormCreatePresset : Form
    {
        public Preset preset { get; private set; }
        public bool savePreset = false;
        public bool success { get; private set; } = false;
        private string presetName = "YOUR_PRESET_NAME";
        private Controller controller;

        public FormCreatePresset(Preset preset, Controller controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.preset = preset;
            this.textBoxPresetName.Text = presetName;
            this.textBoxPresetData.Text = JsonConvert.SerializeObject(this.preset, Newtonsoft.Json.Formatting.Indented);
            this.buttonSave.Enabled = false;
        }

        private void textBoxPresetName_TextChanged(object sender, EventArgs e)
        {
            // trim name from any characters not allowed in json data
            presetName = new string(textBoxPresetName.Text.Where(c => !char.IsControl(c) && c != '"' && c != '\\').ToArray());

            this.preset.title = presetName;
            this.textBoxPresetName.Text = presetName;
            this.textBoxPresetData.Text = JsonConvert.SerializeObject(this.preset, Newtonsoft.Json.Formatting.Indented);

            this.buttonSave.Enabled = (this.presetName != "YOUR_PRESET_NAME" || this.presetName != "");
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.savePreset = true;
            Preset.SaveCustomPresets(Config.GetInstance().CustomPresetsPath, this.preset, controller.user);
            this.success = true;
            Close();
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
