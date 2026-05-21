using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;

namespace STOL_Training_Tool_Core.UI
{
    public partial class FormFirstUser : Form
    {
        private string username = "";

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Username { get => username; private set => username = value; }
        public FormFirstUser()
        {
            InitializeComponent();
        }

        private void linkLabelPrivacyPolicy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/CedricPump/msfs_stol_training_tool/blob/main/doc/Privacy_Policy.md",
                UseShellExecute = true
            });
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            username = textBoxUsername.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            username = textBoxUsername.Text;
        }

        private void textBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // prevent "ding" sound
                buttonSave.Focus();        // move focus to Save button
            }
        }
    }
}
