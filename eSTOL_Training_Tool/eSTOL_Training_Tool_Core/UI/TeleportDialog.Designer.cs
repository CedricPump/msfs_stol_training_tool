namespace STOL_Training_Tool_Core.UI
{
    partial class TeleportDialog
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
            buttonYes = new System.Windows.Forms.Button();
            buttonNo = new System.Windows.Forms.Button();
            buttonNoShow = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // buttonYes
            // 
            buttonYes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonYes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonYes.Location = new System.Drawing.Point(12, 40);
            buttonYes.Name = "buttonYes";
            buttonYes.Size = new System.Drawing.Size(75, 23);
            buttonYes.TabIndex = 0;
            buttonYes.Text = "Yes";
            buttonYes.UseVisualStyleBackColor = true;
            buttonYes.Click += buttonYes_Click;
            // 
            // buttonNo
            // 
            buttonNo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonNo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonNo.Location = new System.Drawing.Point(93, 40);
            buttonNo.Name = "buttonNo";
            buttonNo.Size = new System.Drawing.Size(75, 23);
            buttonNo.TabIndex = 1;
            buttonNo.Text = "No";
            buttonNo.UseVisualStyleBackColor = true;
            buttonNo.Click += buttonNo_Click;
            // 
            // buttonNoShow
            // 
            buttonNoShow.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            buttonNoShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            buttonNoShow.Location = new System.Drawing.Point(174, 40);
            buttonNoShow.Name = "buttonNoShow";
            buttonNoShow.Size = new System.Drawing.Size(159, 23);
            buttonNoShow.TabIndex = 2;
            buttonNoShow.Text = "Yes, do not ask again";
            buttonNoShow.UseVisualStyleBackColor = true;
            buttonNoShow.Click += buttonNoShow_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(263, 15);
            label1.TabIndex = 3;
            label1.Text = "Do you really want to move the aircraft location?";
            // 
            // TeleportDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(359, 75);
            Controls.Add(label1);
            Controls.Add(buttonNoShow);
            Controls.Add(buttonNo);
            Controls.Add(buttonYes);
            Name = "TeleportDialog";
            ShowIcon = false;
            Text = "Teleport";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button buttonYes;
        private System.Windows.Forms.Button buttonNo;
        private System.Windows.Forms.Button buttonNoShow;
        private System.Windows.Forms.Label label1;
    }
}