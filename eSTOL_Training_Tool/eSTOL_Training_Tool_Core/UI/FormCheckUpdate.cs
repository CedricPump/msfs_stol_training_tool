using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STOL_Training_Tool_Core.UI
{
    public partial class FormCheckUpdate : Form
    {
        public bool shouldUpdate = false;
        public string currentVersion = "";
        public string updateVersion = "";

        public FormCheckUpdate(string version = "", string currentVersion = "", bool isUpdateRequired = false)
        {
            InitializeComponent();
            buttonUpdate.Enabled = isUpdateRequired;
            if (isUpdateRequired)
            {
                labelhasVersion.Text = $"An update to version {version} is available. Current version: {currentVersion}";
                linkLabel.Text = $"https://github.com/CedricPump/msfs_stol_training_tool/releases/tag/{version}";
                textBoxVersion.Text = version;
            }
            else
            {
                labelhasVersion.Text = $"Application is up to date: {currentVersion}";
                linkLabel.Text = "https://github.com/CedricPump/msfs_stol_training_tool/releases";
            }
            this.TopLevel = true;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            shouldUpdate = true;
            Close();
        }


        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = linkLabel.Text,
                UseShellExecute = true
            });
        }

        private void textBoxVersion_TextChanged(object sender, EventArgs e)
        {
            if(buttonUpdate.Enabled = textBoxVersion == null || textBoxVersion.Text.Length < 5)
            { 
                buttonUpdate.Enabled = false;
                return; 
            }
            
            if (textBoxVersion.Text.ToLower().StartsWith("http") || textBoxVersion.Text.ToLower().StartsWith("www") || textBoxVersion.Text.ToLower().StartsWith("github.com"))
            {
                try
                {
                    updateVersion = textBoxVersion.Text.Split("/tag/").Last();
                    textBoxVersion.Text = updateVersion;
                    buttonUpdate.Enabled = true;
                    return;
                }
                catch
                {
                    updateVersion = "";
                    buttonUpdate.Enabled = false;
                    return;
                }
            }
            else if (textBoxVersion.Text.ToLower().StartsWith("v"))
            {
                updateVersion = textBoxVersion.Text;
                // textBoxVersion.Text = updateVersion;
                buttonUpdate.Enabled = true;
                return;
            }
            else
            {
                updateVersion = "";
                buttonUpdate.Enabled = false;
                return;
            }
        }
    }
}
