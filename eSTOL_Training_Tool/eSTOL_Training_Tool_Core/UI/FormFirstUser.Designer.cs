namespace STOL_Training_Tool_Core.UI
{
    partial class FormFirstUser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFirstUser));
            buttonSave = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            textBoxUsername = new System.Windows.Forms.TextBox();
            linkLabelPrivacyPolicy = new System.Windows.Forms.LinkLabel();
            SuspendLayout();
            // 
            // buttonSave
            // 
            buttonSave.Location = new System.Drawing.Point(430, 114);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new System.Drawing.Size(75, 23);
            buttonSave.TabIndex = 1;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(490, 90);
            label1.TabIndex = 99;
            label1.Text = resources.GetString("label1.Text");
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new System.Drawing.Point(12, 114);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new System.Drawing.Size(412, 23);
            textBoxUsername.TabIndex = 0;
            textBoxUsername.Text = "Username";
            textBoxUsername.TextChanged += textBoxUsername_TextChanged;
            textBoxUsername.KeyDown += textBoxUsername_KeyDown;
            // 
            // linkLabelPrivacyPolicy
            // 
            linkLabelPrivacyPolicy.AutoSize = true;
            linkLabelPrivacyPolicy.Location = new System.Drawing.Point(425, 84);
            linkLabelPrivacyPolicy.Name = "linkLabelPrivacyPolicy";
            linkLabelPrivacyPolicy.Size = new System.Drawing.Size(80, 15);
            linkLabelPrivacyPolicy.TabIndex = 3;
            linkLabelPrivacyPolicy.TabStop = true;
            linkLabelPrivacyPolicy.Text = "privacy policy";
            linkLabelPrivacyPolicy.LinkClicked += linkLabelPrivacyPolicy_LinkClicked;
            // 
            // FormFirstUser
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(517, 153);
            Controls.Add(linkLabelPrivacyPolicy);
            Controls.Add(textBoxUsername);
            Controls.Add(label1);
            Controls.Add(buttonSave);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "FormFirstUser";
            Text = "First User";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.LinkLabel linkLabelPrivacyPolicy;
    }
}