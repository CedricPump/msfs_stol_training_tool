using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using STOL_Training_Tool_Core.Core;

namespace STOL_Training_Tool_Core.UI
{
    public partial class UpdateDialog : Form
    {
        public bool shouldUpdate = false;

        public UpdateDialog(string newVersion)
        {
            InitializeComponent();
            labeltext.Text = $"A new version ({newVersion}) is available.";
            linkLabel.Text = VersionHelper.githubLatestUrl;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            shouldUpdate = true;
            Close();
        }

        private void InitializeComponent()
        {
            labeltext = new Label();
            linkLabel = new LinkLabel();
            buttonUpdate = new Button();
            buttonSkip = new Button();
            SuspendLayout();
            // 
            // labeltext
            // 
            labeltext.AutoSize = true;
            labeltext.Font = new System.Drawing.Font("Segoe UI", 12F);
            labeltext.Location = new System.Drawing.Point(12, 9);
            labeltext.Name = "labeltext";
            labeltext.Size = new System.Drawing.Size(236, 21);
            labeltext.TabIndex = 0;
            labeltext.Text = "A new version vX.X.X is available.";
            // 
            // linkLabel
            // 
            linkLabel.AutoSize = true;
            linkLabel.Location = new System.Drawing.Point(12, 30);
            linkLabel.Name = "linkLabel";
            linkLabel.Size = new System.Drawing.Size(391, 15);
            linkLabel.TabIndex = 1;
            linkLabel.TabStop = true;
            linkLabel.Text = "https://github.com/CedricPump/msfs_stol_training_tool/releases/latest";
            linkLabel.LinkClicked += linkLabel_LinkClicked;
            // 
            // buttonUpdate
            // 
            buttonUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonUpdate.FlatStyle = FlatStyle.Flat;
            buttonUpdate.Location = new System.Drawing.Point(12, 61);
            buttonUpdate.Name = "buttonUpdate";
            buttonUpdate.Size = new System.Drawing.Size(75, 23);
            buttonUpdate.TabIndex = 2;
            buttonUpdate.Text = "Update";
            buttonUpdate.Click += btnUpdate_Click;
            // 
            // buttonSkip
            // 
            buttonSkip.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonSkip.FlatStyle = FlatStyle.Flat;
            buttonSkip.Location = new System.Drawing.Point(328, 61);
            buttonSkip.Name = "buttonSkip";
            buttonSkip.Size = new System.Drawing.Size(75, 23);
            buttonSkip.TabIndex = 3;
            buttonSkip.Text = "Skip";
            buttonSkip.Click += btnSkip_Click;
            // 
            // UpdateDialog
            // 
            AcceptButton = buttonUpdate;
            ClientSize = new System.Drawing.Size(415, 96);
            Controls.Add(buttonSkip);
            Controls.Add(buttonUpdate);
            Controls.Add(linkLabel);
            Controls.Add(labeltext);
            Name = "UpdateDialog";
            ShowIcon = false;
            TopMost = true;
            Load += UpdateDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            shouldUpdate = false;
            Close();
        }

        private Label labeltext;
        private LinkLabel linkLabel;
        private Button buttonUpdate;
        private Button buttonSkip;

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = linkLabel.Text,
                UseShellExecute = true
            });
        }

        private void UpdateDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
