namespace STOL_Training_Tool_Core.UI
{
    partial class FormCheckUpdate
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
            labelhasVersion = new System.Windows.Forms.Label();
            textBoxVersion = new System.Windows.Forms.TextBox();
            linkLabel = new System.Windows.Forms.LinkLabel();
            label2 = new System.Windows.Forms.Label();
            buttonUpdate = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // labelhasVersion
            // 
            labelhasVersion.AutoSize = true;
            labelhasVersion.Font = new System.Drawing.Font("Segoe UI", 12F);
            labelhasVersion.Location = new System.Drawing.Point(12, 9);
            labelhasVersion.Name = "labelhasVersion";
            labelhasVersion.Size = new System.Drawing.Size(307, 21);
            labelhasVersion.TabIndex = 0;
            labelhasVersion.Text = "A new version is availabe / No new verison";
            // 
            // textBoxVersion
            // 
            textBoxVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            textBoxVersion.Location = new System.Drawing.Point(12, 116);
            textBoxVersion.Name = "textBoxVersion";
            textBoxVersion.Size = new System.Drawing.Size(355, 23);
            textBoxVersion.TabIndex = 2;
            textBoxVersion.TextChanged += textBoxVersion_TextChanged;
            // 
            // linkLabel
            // 
            linkLabel.AutoSize = true;
            linkLabel.Location = new System.Drawing.Point(12, 36);
            linkLabel.Name = "linkLabel";
            linkLabel.Size = new System.Drawing.Size(391, 15);
            linkLabel.TabIndex = 1;
            linkLabel.TabStop = true;
            linkLabel.Text = "https://github.com/CedricPump/msfs_stol_training_tool/releases/latest";
            linkLabel.LinkClicked += linkLabel_LinkClicked;
            // 
            // label2
            // 
            label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 12F);
            label2.Location = new System.Drawing.Point(12, 92);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(449, 21);
            label2.TabIndex = 3;
            label2.Text = "or updated to a verison manually (paste version number or url):";
            // 
            // buttonUpdate
            // 
            buttonUpdate.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            buttonUpdate.Enabled = false;
            buttonUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonUpdate.Font = new System.Drawing.Font("Segoe UI", 9F);
            buttonUpdate.Location = new System.Drawing.Point(373, 116);
            buttonUpdate.Name = "buttonUpdate";
            buttonUpdate.Size = new System.Drawing.Size(88, 23);
            buttonUpdate.TabIndex = 0;
            buttonUpdate.Text = "Update";
            buttonUpdate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            buttonUpdate.UseVisualStyleBackColor = true;
            buttonUpdate.Click += buttonUpdate_Click;
            // 
            // FormCheckUpdate
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(481, 163);
            Controls.Add(buttonUpdate);
            Controls.Add(label2);
            Controls.Add(linkLabel);
            Controls.Add(textBoxVersion);
            Controls.Add(labelhasVersion);
            Name = "FormCheckUpdate";
            ShowIcon = false;
            Text = "Check Update";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelhasVersion;
        private System.Windows.Forms.TextBox textBoxVersion;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonUpdate;
    }
}